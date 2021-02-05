using System;
using System.Collections.Generic;
using System.Text;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{

    public class BookingServiceTest
    {
        private BookingService _bookingService;

        public BookingServiceTest()
        {
            _bookingService = new BookingService();
        }

        [Theory]
        [InlineData("2021-02-06", 2, "2021-02-01", 1, 1)]
        [InlineData("2021-02-05", 1, "2021-02-02", 2, 1)]
        public void BookingService_PostAvailableUnitOnStartDate_ShoulOccupySameUnit(string bookingStartParam, int bookingNightsParam, string bookingStartExisting, int bookingNightsExisting, int rentalAvailableUnits)
        {

            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = rentalAvailableUnits } } };
            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = bookingNightsExisting, RentalId = 1, Start = DateTime.Parse(bookingStartExisting), Unit = 1 } } };


            var model = new BookingBindingModel { Nights = bookingNightsParam, RentalId = 1, Start = DateTime.Parse(bookingStartParam) };

            //act
            var result = _bookingService.CreateBooking(model, rentalDictionary, bookingDictionary);

            //assert
            //same unit
            Assert.Equal(bookingDictionary.Keys.Count, result.Id);
            Assert.Equal(bookingDictionary[bookingDictionary.Keys.Count -1].Unit, bookingDictionary[bookingDictionary.Keys.Count].Unit);
        }

        [Theory]
        [InlineData("2021-02-07", 5, "2021-02-04", 5, 2)]
        [InlineData("2021-02-10", 1, "2021-02-06", 4, 2)]
        [InlineData("2021-02-10", 1, "2021-02-05", 5, 2)]

        public void BookingService_PostAssignNewUnit_ShouldAssignNextAvailableUnit(string bookingStartParam, int bookingNightsParam, string bookingStartExisting, int bookingNightsExisting, int rentalAvailableUnits)
        {
            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = rentalAvailableUnits } } };
            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = bookingNightsExisting, RentalId = 1, Start = DateTime.Parse(bookingStartExisting), Unit = 1 } } };

            var model = new BookingBindingModel { Nights = bookingNightsParam, RentalId = 1, Start = DateTime.Parse(bookingStartParam) };

            //act
            var result = _bookingService.CreateBooking(model, rentalDictionary, bookingDictionary);

            //assert            
            Assert.Equal(bookingDictionary.Keys.Count, result.Id);
            Assert.Equal(rentalAvailableUnits, bookingDictionary[bookingDictionary.Keys.Count].Unit);
        }

        [Theory]
        [InlineData("2021-02-02", 1, "2021-02-01", 1, 1)]
        [InlineData("2021-02-05", 3, "2021-02-07", 1, 1)]

        public void BookingService_PostPrepDayConflict_ShouldThrowError(string bookingStartParam, int bookingNightsParam, string bookingStartExisting, int bookingNightsExisting, int rentalAvailableUnits)
        {

            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>() { { 1, new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = rentalAvailableUnits } } };
            var bookingDictionary = new Dictionary<int, BookingViewModel>() { { 1, new BookingViewModel { Id = 1, Nights = bookingNightsExisting, RentalId = 1, Start = DateTime.Parse(bookingStartExisting), Unit = 1 } } };


            var model = new BookingBindingModel { Nights = bookingNightsParam, RentalId = 1, Start = DateTime.Parse(bookingStartParam) };

            //assert            
            Assert.Throws<ApplicationException>(() =>
            {
                _bookingService.CreateBooking(model, rentalDictionary, bookingDictionary);
            });
        }
    }
}
