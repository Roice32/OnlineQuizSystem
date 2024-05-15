import * as React from 'react'
import Box from '@mui/material/Box'
import Stepper from '@mui/material/Stepper'
import Step from '@mui/material/Step'
import StepLabel from '@mui/material/StepLabel'
import StepContent from '@mui/material/StepContent'
import Button from '@mui/material/Button'
import Paper from '@mui/material/Paper'
import Typography from '@mui/material/Typography'
import Navbar from '../components/Navbar'

import './QuizCreate.css'
import { FormGroup, MenuItem, Select, TextField } from '@mui/material'
import { SelectChangeEvent } from '@mui/material/Select'

import { createTheme } from '@mui/material/styles'
import { ThemeProvider } from '@mui/material/styles'
import { useState } from 'react'

const steps = [
  {
    label: 'Quiz details',
  },
]

type QuestionType = {
  id: number
  text: string
  type: string
  options: string[]
  answer: any[]
}


const theme = createTheme({
  components: {
    MuiTextField: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
          '& .MuiOutlinedInput-input': {
            color: 'white',
          },
          '& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
          '& .MuiInputLabel-outlined': {
            color: 'white',
          },
          '&:hover .MuiOutlinedInput-root .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
          '& .Mui-focused .MuiInputLabel-outlined': {
            color: 'white',
          },
          '& .MuiInputLabel-outlined.Mui-focused': {
            color: 'white',
          },
        },
      },
    },
    MuiSelect: {
      styleOverrides: {
        root: {
          color: 'white',
        },
        iconOutlined: {
          color: 'white',
        },
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
          '&:hover .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
          '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
            borderColor: 'white',
          },
        },
      },
    },
  },
})

export default function VerticalLinearStepper() {
  const [activeStep, setActiveStep] = React.useState(0)
  const [name, setName] = React.useState('')
  const [description, setDescription] = React.useState('')
  const [imageUrl, setImageUrl] = React.useState('')
  const [language, setLanguage] = React.useState('')
  const [timeLimitMinutes, setTimeLimitMinutes] = React.useState(0)
  const [questions, setQuestions] = useState<QuestionType[]>([])

  const handleQuestionChange = (index: number, event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
    const newQuestions = [...questions]
    newQuestions[index].text = event.target.value
    setQuestions(newQuestions)
  }

  const handleAnswerChange = (questionIndex: number, answer: string | number, event: React.ChangeEvent<HTMLInputElement>) => {
    const newQuestions = [...questions]
    const question = newQuestions[questionIndex]

    if (question.type === 'multipleChoice') {
      if (event.target.checked) {
        question.answer.push(answer as number)
      } else {
        const answerIndex = question.answer.indexOf(answer as number)
        if (answerIndex > -1) {
          question.answer.splice(answerIndex, 1)
        }
      }
    } else {
      question.answer = event.target.checked ? [answer as string] : []
    }

    setQuestions(newQuestions)
  }

  const handleQuestionTypeChange = (index: number, event: SelectChangeEvent) => {
    const newQuestions = [...questions]
    newQuestions[index].type = event.target.value as string
    setQuestions(newQuestions)
  }

  const handleOptionChange = (index: number, optionIndex: number, event: React.ChangeEvent<HTMLTextAreaElement | HTMLInputElement>) => {
    const newQuestions = [...questions]
    newQuestions[index].options[optionIndex] = event.target.value
    setQuestions(newQuestions)
  }

  const handleImageUrlChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setImageUrl(event.target.value)
  }

  const handleLanguageChange = (event: SelectChangeEvent) => {
    setLanguage(event.target.value)
  }

  const handleTimeLimitMinutesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setTimeLimitMinutes(Number(event.target.value))
  }

  const handleNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setName(event.target.value)
  }

  const handleDescriptionChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setDescription(event.target.value)
  }

  const handleAddQuestion = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1)

    const newQuestion = {
      id: questions.length + 1,
      text: '',
      type: 'trueFalse',
      options: ['', ''],
      answer: [],
    }

    steps.push({
      label: 'Question ' + (questions.length + 1),
    })

    setQuestions([...questions, newQuestion])

    console.log(questions)
  }

  console.log(activeStep)
  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1)
  }

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1)
  }

  const handleReset = () => {
    setActiveStep(0)
  }

  const renderStepContent = () => {
    if (activeStep == 0) {
      return (
        <FormGroup className="">
          <h1 className="text-[#DEAE9F] text-4xl font-bold mt-6 mb-6">Create Quiz</h1>
          <TextField
            id="name"
            label="Name"
            variant="outlined"
            value={name}
            onChange={handleNameChange}
            sx={{ 'margin-bottom': '30px' }}
          />
          <TextField
            id="description"
            label="Description"
            variant="outlined"
            value={description}
            onChange={handleDescriptionChange}
            sx={{ 'margin-bottom': '30px' }}
          />
          <TextField
            id="imageUrl"
            label="Image URL"
            variant="outlined"
            value={imageUrl}
            onChange={handleImageUrlChange}
            sx={{ 'margin-bottom': '30px' }}
          />
          <Select
            labelId="language-label"
            id="language"
            value={language}
            label="Language"
            onChange={handleLanguageChange}
            sx={{ 'margin-bottom': '30px' }}
          >
            <MenuItem value={'romanian'}>Romanian</MenuItem>
            <MenuItem value={'english'}>English</MenuItem>
          </Select>
          <TextField
            id="timeLimitMinutes"
            label="Time Limit (in minutes)"
            variant="outlined"
            type="number"
            value={timeLimitMinutes}
            onChange={handleTimeLimitMinutesChange}
            sx={{ 'margin-bottom': '30px' }}
          />
        </FormGroup>
      )
    } else {
      const question = questions[activeStep - 1]
      return (
        <FormGroup className="">
          <TextField
            id="question"
            label="Question"
            variant="outlined"
            value={question.text}
            onChange={(event) => handleQuestionChange(activeStep - 1, event)}
            sx={{ 'margin-bottom': '30px' }}
          />
          <Select
            labelId="type-label"
            id="type"
            value={question.type}
            label="Type"
            onChange={(event) => handleQuestionTypeChange(activeStep - 1, event)}
            sx={{ 'margin-bottom': '30px' }}
          >
            <MenuItem value={'trueFalse'}>True/False</MenuItem>
            <MenuItem value={'multipleChoice'}>Multiple Choice</MenuItem>
            <MenuItem value={'singleChoice'}>Single Choice</MenuItem>
            <MenuItem value={'writtenAnswer'}>Written Answer</MenuItem>
          </Select>
          {question.type === 'trueFalse' && (
            <div className="flex flex-col gap-1 mt-3">
              <label className="text-white">
                <input type="radio" value="true" checked={question.options[0] === 'true'}
                       onChange={(e) => handleOptionChange(activeStep - 1, 0, e)} />
                True
              </label>
              <label className="text-white">
                <input type="radio" value="false" checked={question.options[0] === 'false'}
                       onChange={(e) => handleOptionChange(activeStep - 1, 0, e)} />
                False
              </label>
            </div>
          )}
          {question.type === 'singleChoice' && question.options.map((option, index) => (
            <div key={index} className="flex items-center gap-2">
              <input
                type="radio"
                name={`singleChoice${index}`}
                onChange={(e) => handleAnswerChange(activeStep - 1, option, e)}
              />
              <input
                type="text"
                value={option}
                onChange={(e) => handleOptionChange(activeStep - 1, index, e)}
                className="p-2 border border-gray-300 rounded-md"
              />
            </div>
          ))}
          {question.type === 'multipleChoice' && question.options.map((option, index) => (
            <div key={index} className="flex items-center gap-2">
              <input
                type="checkbox"
                checked={question.answer.includes(index)}
                onChange={(e) => handleAnswerChange(activeStep - 1, index, e)}
              />
              <input
                type="text"
                value={option}
                onChange={(e) => handleOptionChange(activeStep - 1, index, e)}
                className="p-2 border border-gray-300 rounded-md"
              />
            </div>
          ))}
          {question.type === 'writtenAnswer' && (
            <input className="p-2 border border-gray-300 rounded-md mt-2" type="text" value={question.options[0]}
                   onChange={(e) => handleOptionChange(activeStep - 1, 0, e)} />
          )}
        </FormGroup>
      )
    }
  }

  return (
    <ThemeProvider theme={theme}>
      <div className="bg-[#0A2D2E] h-screen h-24 max-w-[1240px] mx-auto px-4 text-white">
        <Navbar />
        <Box className="mt-12 h-[80vh] form-quiz-create">
          <Box
            sx={{ maxWidth: 400 }}
            className="overflow-y-scroll form-navigation"
          >
            <Stepper activeStep={activeStep} orientation="vertical">
              {steps.map((step, index) => (
                <Step key={step.label}>
                  <StepLabel>
                    <Typography color="#ffffff">{step.label}</Typography>
                  </StepLabel>
                  <StepContent></StepContent>
                </Step>
              ))}
            </Stepper>
            {activeStep === steps.length && (
              <Paper square elevation={0} sx={{ p: 3 }}>
                <Typography>
                  All steps completed - you&apos;re finished
                </Typography>
                <Button onClick={handleReset} sx={{ mt: 1, mr: 1 }}>
                  Reset
                </Button>
              </Paper>
            )}
          </Box>
          <Box sx={{ mb: 2 }} className="form-buttons">
            <div>
              <Button
                variant="contained"
                onClick={handleAddQuestion}
                sx={{ mt: 1, mr: 1 }}
              >
                Add question
              </Button>
              <Button
                variant="contained"
                onClick={handleNext}
                sx={{ mt: 1, mr: 1 }}
              >
                {activeStep === steps.length - 1 ? 'Finish' : 'Continue'}
              </Button>
              <Button
                disabled={activeStep === 0}
                onClick={handleBack}
                sx={{ mt: 1, mr: 1 }}
              >
                Back
              </Button>
            </div>
          </Box>

          <Box className="form-content flex flex-col gap-6">
            {renderStepContent()}
          </Box>
        </Box>
      </div>
    </ThemeProvider>
  )
}
