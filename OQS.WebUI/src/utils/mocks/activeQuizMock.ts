
import { NakedQuiz } from "../types/naked-quiz";
import { Result } from "../types/result";

export const activeQuizMock:Result<NakedQuiz> = {
    "value": {
    "id": "1af3912f-d625-413a-91b6-cb31f4cbb13b",
    "name": "Quiz 1",
    "timeLimitMinutes": 10,
    "questions": [
      {
        "id": "80116c0e-b841-4dd4-b778-1f094510b8de",
        "type": 1,
        "text": "Question 3",
        "choices": [
          "a",
          "b",
          "c",
          "d"
        ]
      },
      {
        "id": "4c9b58cf-62e9-4cff-9c57-4775e6a3312a",
        "type": 4,
        "text": "Question 5",
      
      },
      {
        "id": "ce66bc07-7002-4d39-82cf-4ea8389a60f7",
        "type": 2,
        "text": "Question 2",
        "choices": [
          "a",
          "b",
          "c",
          "d"
        ]
      },
      {
        "id": "d96829f1-b234-4d6b-94b1-4f09bc403112",
        "type": 0,
        "text": "Question 1",
 
      },
      {
        "id": "42358960-93cd-4045-a17f-7e23006b37f1",
        "type": 3,
        "text": "Question 4",
    
      }
    ]
  },
"isSuccess": true,
"isFailure": false,
"error": {
  "code":200,
"message":"",
}
};