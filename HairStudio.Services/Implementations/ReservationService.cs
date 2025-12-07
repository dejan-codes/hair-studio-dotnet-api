using ClosedXML.Excel;
using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Reservations;
using HairStudio.Services.Enums;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;

namespace HairStudio.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ReservationService(IReservationRepository reservationRepository, IServiceRepository serviceRepository, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _reservationRepository = reservationRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [Auditable("CREATE_USER_RESERVATION")]
        public async Task<Result> CreateUserReservationAsync(short tokenUserId, UserReservationCreateDTO userReservationCreateDTO)
        {
            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var service = await _serviceRepository.GetByIdAsync(userReservationCreateDTO.ServiceId);
            if (service == null || !service.IsActive)
                return Result.Failure(ServiceErrors.ServiceNotFound);

            bool isAdminOrEmployee = user.Roles.Any(r => r.RoleId == (int) RoleEnum.Administrator || r.RoleId == (int) RoleEnum.Employee);
            if (service.Name.Equals("Disable") && !isAdminOrEmployee)
                return Result.Failure(UserErrors.UserNoRoles);

            var reservation = new Reservation
            {
                ServiceId = userReservationCreateDTO.ServiceId,
                ClientUserId = user.UserId,
                ClientCustomerId = null,
                EmployeeId = userReservationCreateDTO.EmployeeId,
                DateFrom = userReservationCreateDTO.DateFrom,
                DateTo = userReservationCreateDTO.DateTo,
                Note = userReservationCreateDTO.Note,
                IsActive = true
            };

            string message = $"User {user.FirstName} {user.LastName} created a reservation from {userReservationCreateDTO.DateFrom} to {userReservationCreateDTO.DateTo}.";

            await _reservationRepository.ExecuteInTransactionAsync(() =>
            {
                _reservationRepository.Add(reservation);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("CREATE_EMPLOYEE_RESERVATION")]
        public async Task<Result> CreateEmployeeReservationAsync(short tokenUserId, EmployeeReservationCreateDTO employeeReservationCreateDTO)
        {
            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var service = await _serviceRepository.GetByIdAsync(employeeReservationCreateDTO.ServiceId);
            if (service == null || !service.IsActive)
                return Result.Failure(ServiceErrors.ServiceNotFound);

            var employee = await _userRepository.GetUserWithRolesAsync(employeeReservationCreateDTO.EmployeeId);
            if (employee == null || !employee.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            bool isAdminOrEmployee = user.Roles.Any(r => r.RoleId == (int)RoleEnum.Administrator || r.RoleId == (int)RoleEnum.Employee);
            if (!isAdminOrEmployee)
                return Result.Failure(UserErrors.UserNoRoles);

            var reservation = new Reservation
            {
                ServiceId = employeeReservationCreateDTO.ServiceId,
                ClientUserId = null,
                EmployeeId = employeeReservationCreateDTO.EmployeeId,
                DateFrom = employeeReservationCreateDTO.DateFrom,
                DateTo = employeeReservationCreateDTO.DateTo,
                Note = employeeReservationCreateDTO.EmployeesNote,
                IsActive = true
            };

            reservation.ClientCustomer = new Customer
            {
                FullName = employeeReservationCreateDTO.FullName,
                Phone = employeeReservationCreateDTO.Phone
            };

            string message = $"Employee {user.FirstName} {user.LastName} created a reservation from {employeeReservationCreateDTO.DateFrom} to {employeeReservationCreateDTO.DateTo} for the client {employeeReservationCreateDTO.FullName}.";

            await _reservationRepository.ExecuteInTransactionAsync(() =>
            {
                _reservationRepository.Add(reservation);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ReservationSummaryDTO>>> GetEmployeeReservationsAsync(short tokenUserId, int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result<IEnumerable<ReservationSummaryDTO>>.Failure(UserErrors.UserNotFound);

            bool isAdminOrEmployee = user.Roles.Any(r => r.RoleId == (int)RoleEnum.Administrator || r.RoleId == (int)RoleEnum.Employee);

            var reservations = await _reservationRepository.GetReservationsByEmployeeAndDateAsync(employeeId, dateFrom, dateTo);

            var selectedReservations = reservations.Select(o => new ReservationSummaryDTO
            {
                ServiceId = o.ServiceId,
                ReservationId = o.ReservationId,
                ServiceName = o.Service?.Name ?? "",
                ClientFullName = o.ClientCustomer != null ? $"{o.ClientCustomer.FullName}" : $"{o.ClientUser.FirstName}" + $"{o.ClientUser.LastName}",
                Start = o.DateFrom,
                End = o.DateTo,
                ShowCancelButton = isAdminOrEmployee || user.UserId == o.ClientUserId
            });

            return Result<IEnumerable<ReservationSummaryDTO>>.Success(selectedReservations);
        }

        public async Task<Result<ReservationDetailsDTO?>> GetReservationDetailsAsync(short tokenUserId, short reservationId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result<ReservationDetailsDTO?>.Failure(UserErrors.UserNotFound);

            bool isAdminOrEmployee = user.Roles.Any(r => r.RoleId == (int)RoleEnum.Administrator || r.RoleId == (int)RoleEnum.Employee);

            var reservation = await _reservationRepository.GetReservationWithDetails(reservationId);
            if (reservation == null)
                return Result<ReservationDetailsDTO?>.Failure(ReservationErrors.ReservationNotFound);

            if (!isAdminOrEmployee && reservation.ClientUserId != user.UserId)
                return Result<ReservationDetailsDTO?>.Failure(UserErrors.UserNoRoles);

            var reservationDetailsDTO = new ReservationDetailsDTO
            {
                ServiceName = reservation.Service.Name,
                ClientFullName = reservation.ClientCustomer != null ? $"{reservation.ClientCustomer.FullName}" : $"{reservation.ClientUser.FirstName}" + " " + $"{reservation.ClientUser.LastName}",
                Start = reservation.DateFrom,
                End = reservation.DateTo,
                PhoneNumber = reservation.ClientUser?.PhoneNumber ?? null,
                Email = reservation.ClientUser != null ? reservation.ClientUser.Email : string.Empty,
                Note = reservation.Note,
            };

            return Result<ReservationDetailsDTO?>.Success(reservationDetailsDTO);
        }

        [Auditable("CANCEL_RESERVATION")]
        public async Task<Result> CancelReservationAsync(short tokenUserId, short reservationId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result<ReservationDetailsDTO?>.Failure(UserErrors.UserNotFound);

            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                return Result.Failure(ReservationErrors.ReservationNotFound);

            bool isAdminOrEmployee = user.Roles.Any(r => r.RoleId == (int) RoleEnum.Administrator || r.RoleId == (int) RoleEnum.Employee);

            if (!isAdminOrEmployee && reservation.ClientUserId != user.UserId)
                return Result<ReservationDetailsDTO?>.Failure(UserErrors.UserNoPermissionForReservationCancellation);

            reservation.IsActive = false;
            string message = $"User {user.FirstName} {user.LastName} cancelled a reservation from {reservation.DateFrom} to {reservation.DateTo}.";

            await _reservationRepository.ExecuteInTransactionAsync(() =>
            {
                _reservationRepository.Update(reservation);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<byte[]> ExportCalendarByEmployeeAsync(DateTime from, DateTime to)
        {
            var employees = await _userRepository.GetEmployeesWithReservationsAsync(from, to);

            using var workbook = new XLWorkbook();

            var timeSlots = Enumerable.Range(0, (int)((20 - 8) * 2))
                .Select(i => TimeSpan.FromMinutes(8 * 60 + i * 30))
                .ToList();

            foreach (var employee in employees)
            {
                var reservations = employee.ReservationEmployees;
                var sheetName = (employee.FirstName + " " + employee.LastName).Length > 31
                    ? (employee.FirstName + " " + employee.LastName).Substring(0, 31)
                    : employee.FirstName + " " + employee.LastName;

                var ws = workbook.Worksheets.Add(sheetName);

                var days = Enumerable.Range(0, (to.Date - from.Date).Days + 1)
                    .Select(offset => from.Date.AddDays(offset))
                    .ToList();

                for (int i = 0; i < days.Count; i++)
                {
                    ws.Cell(1, i + 2).Value = days[i].ToString("dddd, dd.MM");
                    ws.Cell(1, i + 2).Style.Font.Bold = true;
                    ws.Cell(1, i + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                for (int i = 0; i < timeSlots.Count; i++)
                {
                    ws.Cell(i + 2, 1).Value = timeSlots[i].ToString(@"hh\:mm");
                    ws.Cell(i + 2, 1).Style.Font.Bold = true;
                    ws.Cell(i + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(i + 2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                foreach (var res in reservations)
                {
                    var dayOffset = (res.DateFrom.Date - from.Date).Days;
                    if (dayOffset < 0 || dayOffset >= days.Count)
                        continue;

                    var startIndex = timeSlots.FindIndex(t => t == res.DateFrom.TimeOfDay);
                    var endIndex = timeSlots.FindIndex(t => t == res.DateTo.TimeOfDay);

                    if (startIndex == -1 || endIndex == -1)
                        continue;

                    var rowStart = 2 + startIndex;
                    var rowEnd = 2 + endIndex - 1;
                    var col = 2 + dayOffset;

                    var range = ws.Range(rowStart, col, rowEnd, col).Merge();
                    var timeText = $"{res.DateFrom:HH:mm} - {res.DateTo:HH:mm}";
                    var name = res.ClientCustomer != null ? res.ClientCustomer.FullName : res.ClientUser.FirstName + " " + res.ClientUser.LastName;
                    var serviceName = res.Service?.Name == "Disable" ? "Disabled" : res.Service?.Name;
                    var content = $"{timeText}\n{name}\n{res.Service?.Name}";

                    range.Value = content;
                    range.Style.Alignment.WrapText = true;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (serviceName == "Disabled")
                        range.Style.Fill.BackgroundColor = XLColor.LightGray;
                    else
                        range.Style.Fill.BackgroundColor = XLColor.LightBlue;

                    range.Style.Font.FontSize = 11;
                    range.Style.Font.SetBold(false);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                ws.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
