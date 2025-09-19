using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Model;

namespace DataAccessLayer
{
    public interface ITourRepo
    {
        Task<Tour> InsertTourAsync(Tour tour);
        Task EditTourAsync(Tour tour);
        Task DropTourAsync(int tourId);
        Task<List<Tour>> GetAllToursAsync();
    }
}
