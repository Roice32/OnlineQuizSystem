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

const steps = [
  {
    label: 'Quiz details',
  },
]


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
  const [language, setLanguage] = React.useState('romanian')

  const handleChangeLanguage = (event: SelectChangeEvent) => {
    setLanguage(event.target.value)
  }

  console.log(activeStep)
  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1)
    steps.push({
      label: 'Select campaign settings',
    })
  }

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1)
  }

  const handleReset = () => {
    setActiveStep(0)
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
                  <StepLabel
                    optional={
                      index === 2 ? (
                        <Typography variant="caption">Last step</Typography>
                      ) : null
                    }
                  >
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
            <FormGroup className="">
              <TextField id="name-quiz" label="Name" variant="outlined" sx={{ 'margin-bottom': '30px' }} />

              <TextField id="description-quiz" label="Outlined" variant="outlined" sx={{ 'margin-bottom': '30px' }} />

              <Select
                labelId="language-quiz-label"
                id="language-quiz"
                value={language}
                label="Language"
                onChange={handleChangeLanguage}
                inputProps={{ shrink: true }}
                sx={{ 'margin-bottom': '30px' }}
              >
                <MenuItem value={'romanian'}>Romanian</MenuItem>
                <MenuItem value={'english'}>English</MenuItem>
              </Select>
              <TextField inputProps={{ type: 'number' }} sx={{ 'margin-bottom': '30px' }} />
            </FormGroup>
          </Box>
        </Box>
      </div>
    </ThemeProvider>
  )
}
