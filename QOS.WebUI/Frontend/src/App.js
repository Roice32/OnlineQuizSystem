import { BrowserRouter, Routes, Route} from 'react-router-dom';
import Login from './pages/SignIn';
import AfterLogin from './pages/AfterLogin';
import Register from './pages/SignUp';
import SignUpConfirmation from './pages/SignUpConfirmation';
import Profile from './pages/Profile';
import ForgotPassword from './pages/ForgotPassword';
import WaitingToResetPassword from './pages/WaitingToResetPassword';
import ResetPassword from './pages/ResetPassword';

const App = () => {
    return (
      <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register/>} />
        <Route path="/" element={<Login/>} />
        <Route path="/afterLogin" element = {<AfterLogin/>} />
        <Route path="/sign_up_confirmation" element = {<SignUpConfirmation/>} />
        <Route path="/forgot-password" element={<ForgotPassword/>} />
        <Route path="/waiting-to-reset-password" element={<WaitingToResetPassword/>} />
        <Route path="/reset-password/:token" element={<ResetPassword/>} />
      </Routes>
    </BrowserRouter>
    );
};

export default App;