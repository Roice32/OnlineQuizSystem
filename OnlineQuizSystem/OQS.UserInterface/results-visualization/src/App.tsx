import React from "react";
import "./App.css";
import Results from "./Components/Results";

const App: React.FC = () => {
     return <div className="App">
          <span className="heading">Quiz Results</span>
          <Results />
     </div>;
}

export default App;
