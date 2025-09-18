using TourPlanner.Model;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Layout.Borders;
using System.Globalization;

namespace BusinessLayer
{
    public sealed class Report
    {
        public async Task ReportAsync(
        Tour tour,
        List<TourLog> logs,
        byte[]? mapPng,
        string path,
        CancellationToken ct = default)
        {
            try
            {
                await Task.Run(() =>
                {
                    using var writer = new PdfWriter(path);
                    using var pdf = new PdfDocument(writer);
                    using var doc = new Document(pdf);

                    doc.Add(new Paragraph("Tour Report").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));//titel
                    doc.Add(new Paragraph(tour.TourName).SetTextAlignment(TextAlignment.CENTER).SetFontSize(14));//zeigt den namen der tour an
                    doc.Add(new Paragraph("\n"));

                    // Tour details (small table)
                    doc.Add(new Paragraph("Details:").SetFontSize(14));
                    var details = new Table(new float[] { 120, 350 }).UseAllAvailableWidth();
                    void Row(string k, string v)
                    {
                        details.AddCell(new Cell().Add(new Paragraph(k)).SetBorder(Border.NO_BORDER));
                        details.AddCell(new Cell().Add(new Paragraph(v ?? "")).SetBorder(Border.NO_BORDER));
                    }
                    Row("Start:", tour.TourStart);
                    Row("End:", tour.TourEnd);
                    Row("Transport:", tour.Transport);
                    Row("Distance (km):", tour.Distance.HasValue ? tour.Distance.Value.ToString("F1", CultureInfo.InvariantCulture) : "");
                    Row("Estimated Time (min):", tour.EstTime.HasValue ? tour.EstTime.Value.ToString("F0", CultureInfo.InvariantCulture) : "");
                    Row("Description:", tour.TourDescription ?? "");
                    doc.Add(details);
                    doc.Add(new Paragraph("\n"));

                    //Map
                    if (mapPng != null && mapPng.Length > 0)
                    {
                        var imgData = ImageDataFactory.Create(mapPng);
                        var img = new Image(imgData).SetAutoScale(true).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(new Paragraph("Route").SetFontSize(14));
                        doc.Add(img);
                    }

                    Cell CenterCell(string text) => new Cell().Add(new Paragraph(text)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);

                    doc.Add(new Paragraph("Logs:").SetFontSize(14));
                    if (logs != null && logs.Count > 0)
                    {
                        var logTable = new Table(new float[] { 80, 60, 70, 70, 60, 180 }).UseAllAvailableWidth();
                        logTable.AddCell(CenterCell("Datum"));
                        logTable.AddCell(CenterCell("Bewertung"));
                        logTable.AddCell(CenterCell("Schwierigkeit"));
                        logTable.AddCell(CenterCell("Distanz (km)"));
                        logTable.AddCell(CenterCell("Zeit (min)"));
                        logTable.AddCell(CenterCell("Kommentar"));

                        foreach (var log in logs)
                        {
                            logTable.AddCell(CenterCell(log.LogDate.ToString("g")));
                            logTable.AddCell(CenterCell(log.Rating.ToString()));
                            logTable.AddCell(CenterCell(log.LogDifficulty.ToString()));
                            logTable.AddCell(CenterCell(log.LogDistance.ToString("F1")));
                            logTable.AddCell(CenterCell(log.LogTime.ToString("F0")));
                            logTable.AddCell(CenterCell(log.LogComment ?? ""));
                        }
                        doc.Add(logTable);
                    }
                    else
                    {
                        doc.Add(new Paragraph("Keine Logs für diese Tour gefunden."));
                    }
                }, ct);
            }
            catch
            {
                throw new ReportException("Failed to generate report.", ex);
            }
        }
    }
}
