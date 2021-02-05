using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public interface IBookingService
    {
        ResourceIdViewModel CreateBooking(BookingBindingModel model, IDictionary<int, RentalViewModel> rentals, IDictionary<int, BookingViewModel> bookings);
    }
}
