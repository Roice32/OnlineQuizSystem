using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public class FetchQuizResultBodyExtension
    {
        // Command: UserId, QuizId  

        // nu are Validator!!!

        // Handler: bla bla bla pana la Task
        // iei din Db QuizResultBody-yl stocat = qrb
        // pentru fiecare question id pui intr-o lista QuestionBase-ul asociat (din Db de la frunza) = lista 1
        // pentru fiecare { userid, questionid (din qrb-ul gasit) } pui intr-o lista QuestionResultBase-ul asociat (din Db de la noi) = lista 2
        // return new GetQuizResultBodyResponse (lista1, lista2);
    }
}
