using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using VacationRental.Api.Controllers;
using VacationRental.Api.Models;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{
    public class BookingsControllerTest
    {
        private Mock<IBookingService> _bookingService;

        private BookingsController _bookingController;

        public BookingsControllerTest()
        {
            _bookingService = new Mock<IBookingService>();
        }
        
        [Fact]
        public void BookingsController_Post_ShouldCallCreateBooking()
        {
            //arrange
            var model = new BookingBindingModel { Nights = 1, RentalId = 1, Start = DateTime.Now };

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);

            //act
            _bookingController.Post(model);

            //assert
            _bookingService.Verify(bs => bs.CreateBooking(It.IsAny<BookingBindingModel>(), It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<IDictionary<int, BookingViewModel>>()), Times.Once);
        }

        [Fact]
        public void BookingsController_PostNegativeNights_ShouldThrowApplicationException()
        {
            //arrange
            var model = new BookingBindingModel { Nights = -1, RentalId = 2, Start = DateTime.Now };

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);


            //assert
            Assert.Throws<ApplicationException>(() => _bookingController.Post(model));

            _bookingService.Verify(bs => bs.CreateBooking(It.IsAny<BookingBindingModel>(), It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<IDictionary<int, BookingViewModel>>()), Times.Never);
        }

        [Fact]
        public void BookingsController_PostRentalNotFound_ShouldThrowApplicationException()
        {
            //arrange
            var model = new BookingBindingModel { Nights = 1, RentalId = 2, Start = DateTime.Now };

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);


            //assert
            Assert.Throws<ApplicationException>(() => _bookingController.Post(model));

            _bookingService.Verify(bs => bs.CreateBooking(It.IsAny<BookingBindingModel>(), It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<IDictionary<int, BookingViewModel>>()), Times.Never);
        }

        [Fact]
        public void BookingsController_PostPastBooking_ShouldThrowApplicationException()
        {
            //arrange
            var model = new BookingBindingModel { Nights = 1, RentalId = 2, Start = DateTime.Now.AddDays(-2) };

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);


            //assert
            Assert.Throws<ApplicationException>(() => _bookingController.Post(model));

            _bookingService.Verify(bs => bs.CreateBooking(It.IsAny<BookingBindingModel>(), It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<IDictionary<int, BookingViewModel>>()), Times.Never);
        }

        [Fact]
        public void BookingsController_Get_ShouldReturnCorrectBooking()
        {
            //arrange
            var bookingId = 1;

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);

            //Act
            var result = _bookingController.Get(bookingId);


            //assert
            Assert.Equal(bookingDictionary[bookingId].Start, result.Start);
            Assert.Equal(bookingDictionary[bookingId].Id, result.Id);
            Assert.Equal(bookingDictionary[bookingId].RentalId, result.RentalId);
            Assert.Equal(bookingDictionary[bookingId].Unit, result.Unit);
        }


        [Fact]
        public void BookingsController_GetMissingBooking_ShouldThrowApplicationException()
        {
            //arrange
            var bookingId = -1;

            var rentalDictionary = new Dictionary<int, RentalViewModel>();
            rentalDictionary[1] = new RentalViewModel { Id = 1, PreparationTimeInDays = 1, Units = 1 };

            var bookingDictionary = new Dictionary<int, BookingViewModel>();
            bookingDictionary[1] = new BookingViewModel { Id = 1, Nights = 3, RentalId = 1, Start = DateTime.Now, Unit = 1 };

            _bookingController = new BookingsController(rentalDictionary, bookingDictionary, _bookingService.Object);


            //assert
            Assert.Throws<ApplicationException>(() => _bookingController.Get(bookingId));

            _bookingService.Verify(bs => bs.CreateBooking(It.IsAny<BookingBindingModel>(), It.IsAny<IDictionary<int, RentalViewModel>>(), It.IsAny<IDictionary<int, BookingViewModel>>()), Times.Never);
        }
    }
}
