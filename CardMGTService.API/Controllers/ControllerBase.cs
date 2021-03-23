using CardMGTService.Core.Common;
using CardMGTService.Core.Common.Exceptions;
using CardMGTService.Core.Common.Extensions;
using CardMGTService.Core.Dtos;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CardMGTService.API.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        protected readonly NLog.Logger Logger;
        protected string CurrentUserId => User.Identity.GetUserId() ?? "Anonymous"; // TODO: Remove in production

        private string _databaseForeignKeyErrorMessage;
        protected ApiControllerBase(string controllerName)
        {
            Logger = NLog.LogManager.GetLogger(controllerName);
        }

        protected async Task<IApiResponse<T>> HandleApiOperationAsync<T>(Func<Task<DefaultApiReponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
        {
            var apiResponse = new DefaultApiReponse<T> { Code = $"{(int)HttpStatusCode.OK}", ShortDescription = "SUCCESS" };

            var userId = CurrentUserId;

            Logger.Info($" / {method} by {userId} BEGINS.");

            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException("There are some errors in your input, please correct them.", $"{(int)HttpStatusCode.BadRequest}");

                var methodResponse = await action.Invoke();

                apiResponse.Object = methodResponse.Object;
                apiResponse.ShortDescription = string.IsNullOrEmpty(methodResponse.ShortDescription)
                    ? apiResponse.ShortDescription
                    : methodResponse.ShortDescription;
                apiResponse.Code = string.IsNullOrEmpty(methodResponse.Code) ? apiResponse.Code : methodResponse.Code;
            }
            catch (GenericException igex)
            {
                Logger.Warn($"/ {method} L{lineNo} by {userId} - {igex.ErrorCode}: {igex.Message}");
                apiResponse.ShortDescription = igex.Message;
                apiResponse.Code = igex.ErrorCode;

                if (!ModelState.IsValid)
                {
                    apiResponse.ValidationErrors = ModelState.ToDictionary(
                        m =>
                        {
                            var tokens = m.Key.Split('.');
                            return tokens.Length > 0 ? tokens[tokens.Length - 1] : tokens[0];
                        },
                        m => m.Value.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)
                    );
                }
            }
            catch (DbUpdateException duex) when (IsDatabaseDeleteFkException(duex, out _databaseForeignKeyErrorMessage))
            {
                Logger.Warn($"/ {method} L{lineNo} by {userId} - DFK001: {_databaseForeignKeyErrorMessage}");
                apiResponse.ShortDescription = "You cannot delete this record because it is currently in use by one or more records.";
                apiResponse.Code = "DFK001";
            }
            catch (DbEntityValidationException devex) // Should never happen, but is a good way to catch & fix DB validation errors early
            {
                Logger.Warn($"/ {method} L{lineNo} by {userId} - DBV001: {devex.Message}");
                apiResponse.ShortDescription = "Unable to process your request due to one or more data validation errors. Contact tech support for assistance.";
                apiResponse.Code = "DBV001";
            }
            catch (Exception ex)
            {
                Logger.Error($"/ {method} L{lineNo} by {userId} \n\n{ex}\n");
                apiResponse.ShortDescription = Constants.GENERIC_ERROR;
                apiResponse.Code = $"{(int)HttpStatusCode.InternalServerError}";
            }

            Logger.Info($" / {method} by {userId} ENDS.");

            return apiResponse;
        }

        private static bool IsDatabaseDeleteFkException(Exception ex, out string foreignKeyErrorMessage)
        {
            foreignKeyErrorMessage = null;

            var updateEx = ex as DbUpdateException;

            if (updateEx != null && updateEx.Entries.All(e => e.State != EntityState.Deleted))
                return false;

            var exception = ex.InnerException?.InnerException as SqlException;
            var errors = exception?.Errors.Cast<SqlError>();

            var errorMessages = new StringBuilder();

            if (errors != null)
            {
                foreach (var exceptionError in errors.Where(e => e.Number == 547))
                {
                    errorMessages.Append(
                        "Message: " + exceptionError.Message + "\n" +
                        "Error Number: " + exceptionError.Number + "\n" +
                        "LineNumber: " + exceptionError.LineNumber + "\n" +
                        "Source: " + exceptionError.Source + "\n" +
                        "Procedure: " + exceptionError.Procedure + "\n"
                    );
                }
            }

            if (errorMessages.Length == 0) return false;

            foreignKeyErrorMessage = errorMessages.ToString();

            return true;
        }
    }
}

