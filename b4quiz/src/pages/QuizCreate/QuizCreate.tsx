import * as React from 'react'
import Navbar from '../../components/Navbar'
import {
  Card,
  TextField,
  Flex,
  CheckboxGroup,
  RadioGroup,
  Select,
  TextArea,
  Tabs,
  Container,
  Box,
  Text,
  Button,
} from '@radix-ui/themes'

import './QuizCreate.css'

import { useNavigate } from 'react-router-dom'

import { useState } from 'react'
import { Cross1Icon, PlusIcon } from '@radix-ui/react-icons'
import { v4 as uuid } from 'uuid'

const steps = [
  {
    label: 'Quiz details',
  },
]

type Question = {
  inDatabase?: boolean
  id?: string
  text: string
  type: number
  allocatedPoints: number | ''
  timeLimit: number | ''
  trueFalseAnswer?: boolean
  choices?: string[]
  multipleChoiceAnswers?: string[]
  singleChoiceAnswer?: string
  writtenAcceptedAnswers?: string[]
}

enum QuestionType {
  TrueFalse = 0,
  MultipleChoice = 1,
  SingleChoice = 2,
  WrittenAnswer = 3,
}

let QuestionTypeString = {
  0: 'True/False',
  1: 'Multiple Choice',
  2: 'Single Choice',
  3: 'Written Answer',
}

function optionSingleChoice(value: string) {
  return (
    <RadioGroup.Item value={value}>
      <Flex>
        {value}
      </Flex>
    </RadioGroup.Item>
  )

}

function optionMultipleChoice(value: string) {
  return (
    <CheckboxGroup.Item value={value}>
      <Flex>
        {value}
      </Flex>
    </CheckboxGroup.Item>
  )

}

function QuestionComponent({ question, setQuiz, quiz }) {
  const [addOptionMultipleChoice, setAddOptionMultipleChoice] = useState(false)
  const [newOptionMultipleChoice, setNewOptionMultipleChoice] = useState('')
  const [addOptionWrittenAnswer, setAddOptionWrittenAnswer] = useState(false)
  const [newOptionWrittenAnswer, setNewOptionWrittenAnswer] = useState('')

  let questionType = quiz.questions.find((q) => q.id === question.id).type
  let questionText = quiz.questions.find((q) => q.id === question.id).text
  let questionAllocatedPoints = quiz.questions.find((q) => q.id === question.id).allocatedPoints
  let questionTimeLimit = quiz.questions.find((q) => q.id === question.id).timeLimit

  // const [optionsSingleChoice, setOptionsSingleChoice] = useState<OptionChoice[]>([])
  const [addOptionSingleChoice, setAddOptionSingleChoice] = useState(false)
  const [newOptionSingleChoice, setNewOptionSingleChoice] = useState('')
  const [correctAnswerSingleChoice, setCorrectAnswerSingleChoice] = useState('')

  const submitQuestion = (event) => {
    const data = Object.fromEntries(new FormData(event.currentTarget))
    console.log(data)

    event.preventDefault()
  }

  const onChangeQuestion = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            text: event.target.value,
          }
        }
        return q
      }),
    })
  }

  const onChangeQuestionType = (questionTypeValue) => {
    let currentQuestion = quiz.questions.find((q) => q.id === question.id)
    // set the type of current question in quiz
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            type: parseInt(questionTypeValue, 10),
          }
        }
        return q
      }),
    })
  }

  const renderQuestionTypeOptions = () => {
    switch (questionType) {
      case QuestionType.TrueFalse:
        const correctAnswerTrueFalse = quiz.questions.find((q) => q.id === question.id).trueFalseAnswer.toString()

        const setCorrectAnswerTrueFalse = (value) => {
          setQuiz({
            ...quiz,
            questions: quiz.questions.map((q) => {
              if (q.id === question.id) {
                return {
                  ...q,
                  trueFalseAnswer: value === 'true',
                }
              }
              return q
            }),
          })
        }

        return (
          <>
            <Text as="label">Options</Text>
            <RadioGroup.Root value={correctAnswerTrueFalse} name="trueFalse" onValueChange={setCorrectAnswerTrueFalse}>
              <RadioGroup.Item value="true">True</RadioGroup.Item>
              <RadioGroup.Item value="false">False</RadioGroup.Item>
            </RadioGroup.Root>
          </>
        )
      case QuestionType.MultipleChoice:
        // find the current question in the quiz
        let currentQuestion = quiz.questions.find((q) => q.id === question.id)
        let correctAnswers = currentQuestion.multipleChoiceAnswers

        const setCorrectAnswerMultipleChoice = (value) => {
          setQuiz({
            ...quiz,
            questions: quiz.questions.map((q) => {
              if (q.id === question.id) {
                return {
                  ...q,
                  multipleChoiceAnswers: value,
                }
              }
              return q
            }),
          })
        }

        return (
          <>
            <CheckboxGroup.Root value={correctAnswers} onValueChange={setCorrectAnswerMultipleChoice}
                                name="example" mb="10px">
              {question.choices.map(e => optionMultipleChoice(e.value))}
            </CheckboxGroup.Root>
            {addOptionMultipleChoice && (
              <>
                <TextArea mb="10px" placeholder="Option"
                          onChange={(event) => setNewOptionMultipleChoice(event.target.value)} />
                <Flex justify="between">
                  <Button mb="20px" onClick={() => {
                    let newOptionsMultipleChoice = [...question.multipleChoiceAnswers, {
                      value: newOptionMultipleChoice,
                      isCorrect: false,
                    }]
                    setQuiz({
                      ...quiz,
                      questions: quiz.questions.map((q) => {
                        if (q.id === question.id) {
                          return {
                            ...q,
                            choices: newOptionsMultipleChoice,
                          }
                        }
                        return q
                      }),
                    })

                    setAddOptionMultipleChoice(false)
                    setNewOptionMultipleChoice('')
                  }}>
                    <PlusIcon /> Add
                  </Button>
                  <Button color="red" mb="20px" onClick={() => {
                    setAddOptionMultipleChoice(false)
                    setNewOptionMultipleChoice('')
                  }}>
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button onClick={() => {
              setAddOptionMultipleChoice(true)
            }
            }>
              <PlusIcon /> Add Option
            </Button>
          </>
        )
      case QuestionType.SingleChoice:
        return (
          <>
            <RadioGroup.Root value={correctAnswerSingleChoice} onValueChange={setCorrectAnswerSingleChoice}
                             name="example" mb="10px">
              {question.choices.map(e => optionSingleChoice(e.value))}
            </RadioGroup.Root>
            {addOptionSingleChoice && (
              <>
                <TextArea mb="10px" placeholder="Option"
                          onChange={(event) => setNewOptionSingleChoice(event.target.value)} />
                <Flex justify="between">
                  <Button mb="20px" onClick={() => {
                    let newOptionsSingleChoice = [...question.choices, {
                      value: newOptionSingleChoice,
                      isCorrect: false,
                    }]

                    setQuiz({
                      ...quiz,
                      questions: quiz.questions.map((q) => {
                        if (q.id === question.id) {
                          return {
                            ...q,
                            choices: newOptionsSingleChoice,
                          }
                        }
                        return q
                      }),
                    })

                    setAddOptionSingleChoice(false)
                    setNewOptionSingleChoice('')
                  }}>
                    <PlusIcon /> Add
                  </Button>
                  <Button color="red" mb="20px" onClick={() => {
                    setAddOptionSingleChoice(false)
                    setNewOptionSingleChoice('')
                  }}>
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button onClick={() => {
              setAddOptionSingleChoice(true)
            }}>
              <PlusIcon /> Add Option
            </Button>
          </>
        )
      case QuestionType.WrittenAnswer:
        return (
          <>
            <Text mb="10px" as="label">Answers</Text>
            {question.choices && (
              <>
                {question.choices.map((answer, index) => (
                  <Flex direction="column" mb="10px">
                    <Text mb="5px">{answer.value}</Text>
                    <Button onClick={() => {
                      setQuiz({
                        ...quiz,
                        questions: quiz.questions.map((q) => {
                          if (q.id === question.id) {
                            return {
                              ...q,
                              choices: q.choices.filter((a) => a.value !== answer.value),
                            }
                          }
                          return q
                        }),
                      })
                    }}>
                      <Cross1Icon /> Remove
                    </Button>
                  </Flex>
                ))}
              </>
            )}
            {addOptionWrittenAnswer && (
              <>
                <TextArea mb="10px" placeholder="Option"
                          onChange={(event) => setNewOptionWrittenAnswer(event.target.value)} />
                <Flex justify="between">
                  <Button mb="20px" onClick={() => {
                    let newOptionsWrittenAnswer = [...question.choices, {
                      value: newOptionWrittenAnswer,
                      isCorrect: false,
                    }]

                    setQuiz({
                      ...quiz,
                      questions: quiz.questions.map((q) => {
                        if (q.id === question.id) {
                          return {
                            ...q,
                            choices: newOptionsWrittenAnswer,
                          }
                        }
                        return q
                      }),
                    })

                    setQuiz({
                      ...quiz,
                      questions: quiz.questions.map((q) => {
                        if (q.id === question.id) {
                          return {
                            ...q,
                            writtenAcceptedAnswers: newOptionsWrittenAnswer,
                          }
                        }
                        return q
                      }),
                    })

                    setAddOptionWrittenAnswer(false)
                    setNewOptionWrittenAnswer('')
                  }}>
                    <PlusIcon /> Add
                  </Button>
                  <Button color="red" mb="20px" onClick={() => {
                    setAddOptionWrittenAnswer(false)
                    setNewOptionWrittenAnswer('')
                  }}>
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button onClick={() => {
              setAddOptionWrittenAnswer(true)
            }}>
              <PlusIcon /> Add Answer
            </Button>
          </>
        )
    }
  }

  const onChangeAllocatePoints = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            allocatedPoints: parseInt(event.target.value, 10),
          }
        }
        return q
      }),
    })
  }

  const onChangeAllocateTime = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            timeLimit: parseInt(event.target.value, 10),
          }
        }
        return q
      }),
    })
  }


  return (
    <>
      <Flex direction="column" align="center">
        <Flex maxWidth="400px" direction="column">
          <Box mb="10px">
            <Text as="label">Question</Text>
            <TextArea placeholder="Add a question…" value={questionText} onChange={onChangeQuestion} />
          </Box>
          <Box mb="10px">
            <Text as="label">Allocated Points</Text>
            <TextField.Root placeholder="Add a allocated points..." value={questionAllocatedPoints}
                            onChange={onChangeAllocatePoints} />
          </Box>
          <Box mb="10px">
            <Text as="label">Time Limit</Text>
            <TextField.Root placeholder="Add a time limit..." value={questionTimeLimit}
                            onChange={onChangeAllocateTime} />
          </Box>
          <Flex direction="column" mb="10px">
            <Text as="label">Type</Text>
            <Select.Root value={QuestionType[questionType]} onValueChange={onChangeQuestionType}>
              <Select.Trigger>{QuestionType[questionType]}</Select.Trigger>
              <Select.Content>
                {Object.keys(QuestionTypeString).map((key) => (
                  <Select.Item key={key} value={key}>
                    {QuestionTypeString[key]}
                  </Select.Item>
                ))}
              </Select.Content>
            </Select.Root>
          </Flex>
          <Flex direction="column" mb="10px">
            {renderQuestionTypeOptions()}
          </Flex>

          <Button mt="20px" onClick={() => {
            setQuiz({
              ...quiz,
              questions: quiz.questions.filter((q) => q.id !== question.id),
            })
          }}>
            Remove Question
          </Button>

        </Flex>
      </Flex>
    </>
  )
}

type QuizCreateProps = {
  id?: string
  inDatabase?: boolean
  name?: string
  imageUrl?: string
  description?: string
  timeLimit?: number | ''
  language?: string
  questions?: Question[]
}

type ErrorMessagesQuizCreate = {
  name?: string
  imageUrl?: string
  description?: string
  timeLimit?: string
  language?: string
  questions?: string
}

let ErrorMessagesQuizCreateString = [
  'name', 'imageUrl', 'description', 'timeLimit', 'language', 'questions',
]


export default function QuizCreate() {
  const navigate = useNavigate()
  const [quiz, setQuiz] = useState<QuizCreateProps>({ language: 'romanian' })
  const [errorMessages, setErrorMessages] = useState<ErrorMessagesQuizCreate>({})

  return (
    <>
      <Navbar />
      <Container p="30px">
        <Tabs.Root defaultValue="quiz-details">
          <Flex justify="between">
            <Tabs.List size={'2'} wrap={'wrap'}>
              <Tabs.Trigger value="quiz-details">Quiz details</Tabs.Trigger>
              {quiz.questions && quiz.questions.map((question, index) => (<>
                <Tabs.Trigger value={`question${index}`}>Question {index + 1}</Tabs.Trigger>
              </>))}
              <Tabs.Trigger value="submit-quiz">Submit Quiz</Tabs.Trigger>
            </Tabs.List>

            <Button onClick={() => {
              if (!quiz.questions) {
                setQuiz({
                  ...quiz, 'questions': [{
                    id: uuid(),
                    text: '',
                    type: 0,
                    allocatedPoints: 0,
                    trueFalseAnswer: true,
                    choices: [],
                    multipleChoiceAnswers: [],
                    writtenAcceptedAnswers: [],
                    timeLimit: 0,
                  }],
                })
              } else {
                setQuiz({
                  ...quiz,
                  questions: [...quiz.questions, {
                    id: uuid(),
                    text: '',
                    type: 0,
                    choices: [],
                    trueFalseAnswer: true,
                    multipleChoiceAnswers: [],
                    writtenAcceptedAnswers: [],
                    allocatedPoints: 0,
                    timeLimit: 0,
                  }],
                })
              }
            }}>Add Question</Button>
          </Flex>

          <Box pt="3" width="100%">
            <Tabs.Content value="quiz-details">
              <Flex direction="column" align="center">
                <Flex maxWidth="400px" direction="column">
                  <Box mb="10px">
                    <Text as="label">Name</Text>
                    <TextField.Root placeholder="Add a name..." value={quiz.name} onChange={(event) => {
                      if (!event.target.value) {
                        if (event.target.value === '') {
                          setQuiz({ ...quiz, 'name': event.target.value })
                        }
                        setErrorMessages({ ...errorMessages, 'name': 'Name is required' })
                      } else {
                        setQuiz({ ...quiz, 'name': event.target.value })
                        setErrorMessages({ ...errorMessages, 'name': '' })
                      }
                    }} />
                    {errorMessages.name && <Text as="p" color="red">{errorMessages.name}</Text>}
                  </Box>
                  <Box mb="10px">
                    <Text as="label">Image URL</Text>
                    <TextField.Root placeholder="Add a image url..." value={quiz.imageUrl} onChange={(event) => {
                      if (!event.target.value) {
                        if (event.target.value === '') {
                          setQuiz({ ...quiz, 'imageUrl': event.target.value })
                        }
                        setErrorMessages({ ...errorMessages, 'imageUrl': 'Image URL is required' })
                      } else {
                        setQuiz({ ...quiz, 'imageUrl': event.target.value })
                        setErrorMessages({ ...errorMessages, 'imageUrl': '' })
                      }
                    }} />
                    {errorMessages.imageUrl && <Text as="p" color="red">{errorMessages.imageUrl}</Text>}
                  </Box>
                  <Box mb="10px">
                    <Text as="label">Description</Text>
                    <TextArea placeholder="Add a description..." value={quiz.description} onChange={(event) => {
                      if (!event.target.value) {
                        if (event.target.value === '') {
                          setQuiz({ ...quiz, 'description': event.target.value })
                        }
                        setErrorMessages({ ...errorMessages, 'description': 'Description is required' })
                      } else {
                        setQuiz({ ...quiz, 'description': event.target.value })
                        setErrorMessages({ ...errorMessages, 'description': '' })
                      }
                    }} />
                    {errorMessages.description && <Text as="p" color="red">{errorMessages.description}</Text>}
                  </Box>
                  <Flex mb="10px" direction="column">
                    <Text as="label">Language</Text>
                    <Select.Root defaultValue="romanian" value={quiz.language} onValueChange={(value) => {
                      setQuiz({ ...quiz, 'language': value })
                    }}>
                      <Select.Trigger />
                      <Select.Content>
                        <Select.Item value="romanian">Romanian</Select.Item>
                        <Select.Item value="english">English</Select.Item>
                        <Select.Item value="spanish">Spanish</Select.Item>
                        <Select.Item value="french">French</Select.Item>
                      </Select.Content>
                    </Select.Root>
                  </Flex>
                  <Box mb="10px">
                    <Text as="label">Time limit</Text>
                    <TextField.Root placeholder="Add a time limit..." value={quiz.timeLimit} onChange={(event) => {
                      // verify if a string is a number in js

                      if (!isNaN(Number(event.target.value)) && Number(event.target.value) > 0) {
                        setQuiz({ ...quiz, 'timeLimit': Number(event.target.value) })
                        setErrorMessages({ ...errorMessages, 'timeLimit': '' })
                      } else {
                        if ('' === event.target.value) {
                          setQuiz({ ...quiz, 'timeLimit': event.target.value })
                        }
                        setErrorMessages({ ...errorMessages, 'timeLimit': 'Invalid time limit' })
                      }
                    }} />
                    {errorMessages.timeLimit && <Text as="p" color="red">{errorMessages.timeLimit}</Text>}
                  </Box>
                </Flex>
              </Flex>
            </Tabs.Content>

            {quiz.questions && quiz.questions.map((question, index) => (<>
              <Tabs.Content value={`question${index}`}>
                <QuestionComponent question={question} setQuiz={setQuiz} quiz={quiz} />
              </Tabs.Content>
            </>))}

            <Tabs.Content value="submit-quiz">
              <Flex justify="center">
                <Flex direction="column" justify="center" maxWidth="500px">
                  <Text mb="20px">If you are ready you can submit your quiz</Text>
                  <Button onClick={() => {
                    let quizRequest = {
                      name: quiz.name,
                      imageUrl: quiz.imageUrl,
                      timeLimitMinutes: quiz.timeLimit,
                      language: quiz.language,
                      description: quiz.description,
                      creatorId: '822c13a0-8872-431e-ad64-00f5249db11f',
                    }

                    console.log(quiz)
                    console.log(quizRequest)
                    fetch('http://localhost:5276/api/quizzes', {
                      method: 'POST',
                      headers: {
                        'Content-Type': 'application/json',
                      },
                      body: JSON.stringify(quizRequest),
                    })
                      .then((response) =>
                        response.json(),
                      )
                      .then((data) => {
                        if (typeof data === 'string') {
                          return navigate(data.substring(4, data.length))
                        }

                        console.log(data)
                        if (data.code !== 200) {
                          let errorMessagesStrings = data?.message.split('\n')
                          let errorMessages = {}
                          for (let error of errorMessagesStrings) {
                            if (error.toLowerCase().includes('name')) {
                              errorMessages = { ...errorMessages, 'name': error }
                            }
                            if (error.toLowerCase().includes('image')) {
                              errorMessages = { ...errorMessages, 'imageUrl': error }
                            }
                            if (error.toLowerCase().includes('description')) {
                              errorMessages = { ...errorMessages, 'description': error }
                            }
                            if (error.toLowerCase().includes('time')) {
                              errorMessages = { ...errorMessages, 'timeLimit': error }
                            }
                            if (error.toLowerCase().includes('language')) {
                              errorMessages = { ...errorMessages, 'language': error }
                            }
                          }

                          setErrorMessages({ ...errorMessages })
                        }

                        console.log('Success:', data)
                      })
                      .catch((error) => {
                        console.error('Error:', error)
                      })
                  }}>
                    Submit Quiz
                  </Button>
                </Flex>
              </Flex>
            </Tabs.Content>
          </Box>
        </Tabs.Root>
      </Container>
    </>
  )
}
