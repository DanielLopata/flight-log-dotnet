namespace FlightLogNet.Operation
{
    using System.Text;

    using Models;
    using Repositories.Interfaces;

    public class GetExportToCsvOperation(IFlightRepository flightRepository)
    {
        public byte[] Execute()
        {
            var report = flightRepository.GetReport();
            var csv = new StringBuilder();

            csv.AppendLine(
                $"{ReportLocalization.FlightIdColumn},{ReportLocalization.DateColumn},{ReportLocalization.TakeoffTimeColumn},{ReportLocalization.LandingTimeColumn},{ReportLocalization.ImmatriculationColumn},{ReportLocalization.TypeColumn},{ReportLocalization.PilotColumn},{ReportLocalization.CopilotColumn},{ReportLocalization.TaskColumn},{ReportLocalization.TowplaneIdColumn},{ReportLocalization.GliderIdColumn}");

            foreach (var reportModel in report)
            {
                AppendFlight(csv, reportModel.Towplane, reportModel.Towplane?.Id, reportModel.Glider?.Id);
                AppendFlight(csv, reportModel.Glider, reportModel.Towplane?.Id, reportModel.Glider?.Id);
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private static void AppendFlight(StringBuilder builder, FlightModel flight, long? towplaneId, long? gliderId)
        {
            if (flight == null)
            {
                return;
            }

            var copilot = flight.Copilot == null ? string.Empty : $"{flight.Copilot.FirstName} {flight.Copilot.LastName}";
            var pilot = flight.Pilot == null ? string.Empty : $"{flight.Pilot.FirstName} {flight.Pilot.LastName}";
            var date = flight.TakeoffTime.ToString("dd.MM.yyyy");
            var takeoff = flight.TakeoffTime.ToString("HH:mm:ss");
            var landing = flight.LandingTime?.ToString("dd.MM.yyyy HH:mm:ss") ?? string.Empty;

            builder.AppendLine(
                $"{flight.Id},{date},{takeoff},{landing},{flight.Airplane?.Immatriculation},{flight.Airplane?.Type},{pilot},{copilot},{flight.Task},{towplaneId},{gliderId}");
        }
    }
}
