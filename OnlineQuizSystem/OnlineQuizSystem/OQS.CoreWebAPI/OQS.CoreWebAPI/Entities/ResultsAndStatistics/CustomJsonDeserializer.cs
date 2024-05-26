using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Temp;

namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics
{
    public class CustomJsonDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(QuestionBase) ||
                objectType == typeof(QuestionAnswerPairBase) ||
                objectType == typeof(QuestionResultBase);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (objectType == typeof(QuestionAnswerPairBase))
            {
                return DeserializeQuestionAnswerPair(jo);
            }
            else if (objectType == typeof(QuestionResultBase))
            {
                return DeserializeQuestionResult(jo);
            }
            return DeserializeQuestion(jo);
        }

        public QuestionBase DeserializeQuestion(JObject jo)
        {
            if (jo["TrueFalseAnswer"] != null)
            {
                return jo.ToObject<TrueFalseQuestion>();
            }
            else if (jo["SingleChoiceAnswer"] != null)
            {
                return jo.ToObject<SingleChoiceQuestion>();
            }
            else if (jo["MultipleChoiceAnswers"] != null)
            {
                return jo.ToObject<MultipleChoiceQuestion>();
            }
            else if (jo["WrittenAcceptedAnswers"] != null)
            {
                return jo.ToObject<WrittenAnswerQuestion>();
            }
            return jo.ToObject<ReviewNeededQuestion>();
        }

        public QuestionAnswerPairBase DeserializeQuestionAnswerPair(JObject jo)
        {
            if (jo["TrueFalseAnswer"] != null)
            {
                return jo.ToObject<TrueFalseQAPair>();
            }
            else if (jo["SingleChoiceAnswer"] != null)
            {
                return jo.ToObject<SingleChoiceQAPair>();
            }
            else if (jo["MultipleChoiceAnswers"] != null)
            {
                return jo.ToObject<MultipleChoiceQAPair>();
            }
            else if (jo["WrittenAnswer"] != null)
            {
                return jo.ToObject<WrittenQAPair>();
            }
            return null;
        }

        public QuestionResultBase DeserializeQuestionResult(JObject jo)
        {
            if (jo["TrueFalseAnswerResult"] != null)
            {
                return jo.ToObject<TrueFalseQuestionResult>();
            }
            else if (jo["PseudoDictionaryChoicesResults"] != null)
            {
                return jo.ToObject<ChoiceQuestionResult>();
            }
            else if (jo["WrittenAnswerResult"] != null)
            {
                return jo.ToObject<WrittenAnswerQuestionResult>();
            }
            else if (jo["ReviewNeededResult"] != null)
            {
                return jo.ToObject<ReviewNeededQuestionResult>();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
