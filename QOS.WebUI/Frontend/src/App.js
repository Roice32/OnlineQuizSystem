import { BrowserRouter, Routes, Route} from 'react-router-dom';
import Login from './pages/SignIn';
import AfterLogin from './pages/AfterLogin';
import Register from './pages/SignUp';
import SignUpConfirmation from './pages/SignUpConfirmation';

const App = () => {
    return (
      <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register/>} />
        <Route path="/" element={<Login/>} />
        <Route path="/afterLogin" element = {<AfterLogin/>} />
        <Route path="/sign_up_confirmation" element = {<SignUpConfirmation/>} />
      </Routes>
    </BrowserRouter>
    );
};

export default App;