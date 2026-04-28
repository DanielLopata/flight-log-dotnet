namespace FlightLogNet.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models;
    using Entities;
    using Interfaces;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class FlightRepository(IConfiguration configuration) : IFlightRepository
    {
        public IList<FlightModel> GetFlightsOfType(FlightType type)
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flights = dbContext.Flights
                .Where(flight => flight.Type == type)
                .Include(flight => flight.Airplane).ThenInclude(airplane => airplane.ClubAirplane).ThenInclude(ca => ca.AirplaneType)
                .Include(flight => flight.Pilot).ThenInclude(person => person.Address)
                .Include(flight => flight.Copilot).ThenInclude(person => person.Address);

            return flights.Select(f => f.ToModel()).ToList();
        }

        public IList<FlightModel> GetAllFlights()
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flights = dbContext.Flights
                .Include(flight => flight.Airplane).ThenInclude(airplane => airplane.ClubAirplane).ThenInclude(ca => ca.AirplaneType)
                .Include(flight => flight.Pilot).ThenInclude(person => person.Address)
                .Include(flight => flight.Copilot).ThenInclude(person => person.Address);

            return flights.Select(f => f.ToModel()).ToList();
        }

        public IList<FlightModel> GetAirplanesInAir()
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flightsInAir = dbContext.Flights
                .Where(flight => flight.LandingTime == null)
                .Include(flight => flight.Airplane).ThenInclude(airplane => airplane.ClubAirplane).ThenInclude(ca => ca.AirplaneType)
                .Include(flight => flight.Pilot).ThenInclude(person => person.Address)
                .Include(flight => flight.Copilot).ThenInclude(person => person.Address)
                .OrderBy(flight => flight.TakeoffTime)
                .ThenBy(flight => flight.Type == FlightType.Towplane ? 0 : 1);

            return flightsInAir.Select(f => f.ToModel()).ToList();
        }

        public void LandFlight(FlightLandingModel landingModel)
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flight = dbContext.Flights.Find(landingModel.FlightId) 
                         ?? throw new NotSupportedException($"Unable to land not-registered flight: {landingModel}.");
            flight.LandingTime = landingModel.LandingTime;
            dbContext.SaveChanges();
        }

        public void TakeoffFlight(long? gliderFlightId, long? towplaneFlightId)
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flightStart = new FlightStart
            {
                Glider = dbContext.Flights.Find(gliderFlightId),
                Towplane = dbContext.Flights.Find(towplaneFlightId),
            };

            dbContext.FlightStarts.Add(flightStart);
            dbContext.SaveChanges();
        }

        public long CreateFlight(CreateFlightModel model)
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var copilot = model.CopilotId != null
                ? dbContext.Persons.Find(model.CopilotId)
                : null;

            var flight = new Flight
            {
                Airplane = dbContext.Airplanes.Find(model.AirplaneId),
                Copilot = copilot,
                Pilot = dbContext.Persons.Find(model.PilotId),
                TakeoffTime = model.TakeOffTime,
                Task = model.Task
            };

            dbContext.Flights.Add(flight);
            dbContext.SaveChanges();

            return flight.Id;
        }

        public IList<ReportModel> GetReport()
        {
            using var dbContext = new LocalDatabaseContext(configuration);

            var flightStarts = dbContext.FlightStarts
                .Include(flight => flight.Glider)
                .Include(flight => flight.Glider.Airplane)
                .Include(flight => flight.Glider.Pilot)
                .Include(flight => flight.Glider.Copilot)
                .Include(flight => flight.Towplane)
                .Include(flight => flight.Towplane.Airplane)
                .Include(flight => flight.Towplane.Pilot)
                .Include(flight => flight.Towplane.Copilot)
                .OrderByDescending(start => start.Towplane.TakeoffTime);

            return flightStarts.Select(fs => fs.ToModel()).ToList();
        }
    }
}
