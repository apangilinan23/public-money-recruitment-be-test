using System;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public class BookingService : IBookingService
    {
        public ResourceIdViewModel CreateBooking(BookingBindingModel model, IDictionary<int, RentalViewModel> rentals, IDictionary<int, BookingViewModel> bookings)
        {
            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in bookings.Values)
                {
                    if (booking.RentalId == model.RentalId
                        && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                        || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                        || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                    {
                        count++;
                    }
                }

                if (count >= rentals[model.RentalId].Units)
                    throw new ApplicationException("Not available");
            }
            var key = new ResourceIdViewModel { Id = bookings.Keys.Count + 1 };
            var newBooking = new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date,
                Unit = key.Id
            };

            if (newBooking.Unit > rentals[model.RentalId].Units)
                throw new ApplicationException("Not available");
            else
                bookings.Add(key.Id, newBooking);

            return key;
        }
    }
}
