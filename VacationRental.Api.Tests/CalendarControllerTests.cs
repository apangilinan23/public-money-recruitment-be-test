using Moq;
using System;
using System.Collections.Generic;
using VacationRental.Api.Controllers;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{

    public class CalendarControllerTests
    {
        private Mock<ICalendarService> _calendarService;

        private CalendarController _calendarController;

        public CalendarControllerTests()
        {
            _calendarService = new Mock<ICalendarService>();
        }

        [Fact]
        public void CalendarController_Get_ShouldCallCalendarServiceGetCalendarViewModelResult()
        {
            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _calendarController = new CalendarController(rentalDictionary, bookingDictionary, _calendarService.Object);


            //act
            _calendarController.Get(1, DateTime.Now, 1);

            //assert
            _calendarService.Verify(x => x.GetCalendarViewModelResult(It.IsAny<IDictionary<int, BookingViewModel>>(),
                It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void CalendarController_NightsLessThanZero_ShouldThrowApplicationException()
        {
            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _calendarController = new CalendarController(rentalDictionary, bookingDictionary, _calendarService.Object);


            //assert
            Assert.Throws<ApplicationException>(() => _calendarController.Get(1, DateTime.Now, -1));

            _calendarService.Verify(x => x.GetCalendarViewModelResult(It.IsAny<IDictionary<int, BookingViewModel>>(),
                It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void CalendarController_RentalNotFound_ShouldThrowApplicationException()
        {
            //arrange
            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _calendarController = new CalendarController(rentalDictionary, bookingDictionary, _calendarService.Object);

            //assert
            Assert.Throws<ApplicationException>(() => _calendarController.Get(10, DateTime.Now, 1));

            _calendarService.Verify(x => x.GetCalendarViewModelResult(It.IsAny<IDictionary<int, BookingViewModel>>(),
                It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
