using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Common
{
    //Result Pattern ===> insread of throwing exeption or returning null (service will return small object says the message) "the caller is force to check before calling the value"
    public sealed record Result (bool success , string? error=null , ResultKind kind = ResultKind.OK)
    {
        //ResultKind "Enum" icluded  types of fauiler that's send
        //sealed "not inherted"
        

        //------------------------- For Services That Return True Or False-----------------------------------------------
        public static Result OK()=> new Result(success : true);
        public static Result Fail(string error,ResultKind kind = ResultKind.Confilct)=> new Result (false, error, kind);
        public static Result Fail(string error = "Not Found") => new(false, error, ResultKind.NotFound);
        public static Result Validation(string error) => new(false, error, ResultKind.ValidationFailed);

    }

    //------------------------- For Services That Return "Date" (IEnumrable<>)-----------------------------------------------
    public sealed record Result<T>(bool success , T? value , string? error=null ,ResultKind Kind =ResultKind.OK)
    {
        public static Result<T> OK(T value) => new ( true, value);
        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Confilct) => new (false, default,error, kind);
        public static Result<T> NotFound(string error = "Not Found") => new(false,default ,error, ResultKind.NotFound);
        public static Result<T> Validation(string error) => new(false,default , error, ResultKind.ValidationFailed);

    }
}
