import * as React from "react";
import Box from "@mui/material/Box";
import Stepper from "@mui/material/Stepper";
import Step from "@mui/material/Step";
import StepLabel from "@mui/material/StepLabel";
import StepContent from "@mui/material/StepContent";
import Button from "@mui/material/Button";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import Navbar from "../components/Navbar";

import "./QuizCreate.css";
import { TextField } from "@mui/material";

const steps = [
  {
    label: "Select campaign settings",
  },
];

export default function VerticalLinearStepper() {
  const [activeStep, setActiveStep] = React.useState(0);

  console.log(activeStep);
  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
    steps.push({
      label: "Select campaign settings",
    });
  };

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  const handleReset = () => {
    setActiveStep(0);
  };

  return (
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
              {activeStep === steps.length - 1 ? "Finish" : "Continue"}
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
          <TextField id="outlined-basic" label="Outlined" variant="outlined" />
          <TextField id="outlined-basic" label="Outlined" variant="outlined" />
          <TextField id="outlined-basic" label="Outlined" variant="outlined" />
        </Box>
      </Box>
    </div>
  );
}
