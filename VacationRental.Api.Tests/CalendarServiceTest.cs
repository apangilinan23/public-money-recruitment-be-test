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

        [Theory]
        [InlineData("2021-04-02", "2021-04-02", 2, 1, 1)]
        [InlineData("2021-04-02", "2021-04-02", 2, 1, 5)]
        [InlineData("2021-04-01", "2021-04-02", 30, 1, 5)]
        [InlineData("2021-04-03", "2021-04-05", 4, 1, 5)]
        public void CalendarService_GetHappyPath_ShouldReturnCalendarViewModel(string calendarStartDateString , string bookingStartDateString, int datesToDisplay, int nights, int preparationTimeInDays)
        {
            //arrange
            var calendarResult = new CalendarViewModel
            {
                RentalId = 1,
                Dates = new List<CalendarDateViewModel>()
            };
            var bookingStartDate = DateTime.Parse(bookingStartDateString);
            var calendarStartDate = DateTime.Parse(calendarStartDateString);

            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = preparationTimeInDays, Units = 1 } } };
            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = 1, RentalId = 1, Start = bookingStartDate, Unit = 1 } } };    

            //act
            var result = _calendarService.GetCalendarViewModelResult(bookingDictionary, rentalDictionary, datesToDisplay, calendarResult.RentalId, calendarStartDate);
            var lastBookingDate = result.Dates.FirstOrDefault(d => d.Date == bookingStartDate.AddDays(nights - 1));

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Dates);

            //booking start date
            Assert.NotNull(result.Dates.FirstOrDefault(d => d.Date == bookingStartDate));
            Assert.Equal(result.Dates.FirstOrDefault(d => d.Date == bookingStartDate)?.Date, bookingStartDate);

            //prep time
            Assert.NotEmpty(result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date.AddDays(1))?.PreparationTimes);
            Assert.Empty(result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date.AddDays(1)).Bookings);

            //Unit no of prep time vs booking
            Assert.Equal(result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date.AddDays(1)).PreparationTimes.FirstOrDefault().Unit,
                result.Dates.FirstOrDefault(d => d.Date == lastBookingDate.Date).Bookings.FirstOrDefault().Unit);

            //no of days
            Assert.Equal(datesToDisplay, result.Dates.Count);
        }
    }
}
