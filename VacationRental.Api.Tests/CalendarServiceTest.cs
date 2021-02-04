using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{
    public class CalendarServiceTest
    {
        private CalendarService _calendarService;
        public CalendarServiceTest() 
        {
            _calendarService = new CalendarService();
        }

        [Fact]
        public void CalendarService_Get_ShouldReturnCalendarViewModel()
        {
            //arrange
            var calendarResult = new CalendarViewModel
            {
                RentalId = 1,
                Dates = new List<CalendarDateViewModel>()
            };
            var bookingStartDate = DateTime.Today;
            int datesToDisplay = 2;
            var nights = 1;

            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 } } };
            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = 1, RentalId = 1, Start = bookingStartDate, Unit = 1 } } };    

            //act
            var result = _calendarService.GetCalendarViewModelResult(bookingDictionary, rentalDictionary, datesToDisplay, calendarResult.RentalId, bookingStartDate);

            var lastBookingDate = result.Dates.FirstOrDefault(d => d.Date == bookingStartDate.AddDays(nights - 1));
            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Dates);

            Assert.Equal(result.Dates.FirstOrDefault()?.Date, bookingStartDate);
            Assert.Empty(result.Dates.FirstOrDefault()?.PreparationTimes);

            Assert.Empty(result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date.AddDays(1)).Bookings);

            Assert.Equal(result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date.AddDays(1)).PreparationTimes.FirstOrDefault().Unit,
                result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date).Bookings.FirstOrDefault().Unit);
        }

        [Fact]
        public void CalendarService_GetShouldPopulatePrepTime_ShouldReturnCalendarViewModelWithPrepTime()
        {
            //arrange
            var calendarResult = new CalendarViewModel
            {
                RentalId = 1,
                Dates = new List<CalendarDateViewModel>()
            };

            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 } } };


            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = 10, RentalId = 1, Start = DateTime.Now, Unit = 1 } } };

            //act
            var result = _calendarService.GetCalendarViewModelResult(bookingDictionary, rentalDictionary, 1, calendarResult.RentalId, DateTime.Today.AddDays(5));

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Dates);
            Assert.Equal(result.Dates[0].Date, DateTime.Today.AddDays(5));
            Assert.Equal(result.RentalId, rentalDictionary[1].Id);
        }
    }
}
