import { BrowserRouter, Routes, Route} from 'react-router-dom';
import Login from './pages/SignIn';
import AfterLogin from './pages/AfterLogin';
import Register from './pages/SignUp';
import SignUpConfirmation from './pages/SignUpConfirmation';
import Profile from './pages/Profile';
import ForgotPassword from './pages/ForgotPassword';
import WaitingToResetPassword from './pages/WaitingToResetPassword';
import ResetPassword from './pages/ResetPassword';
import EditProfile from './pages/EditProfile';
import SignIn from './pages/SignIn';
import Logout from './pages/Logout';
import VerifyCurrentPassword from './pages/VerifyCurrentPassword';
import ResetCurrentPasseword from './pages/ResetPasswordFromProfilPage';
import AddAdmin from './pages/AddAdmin';
import GetUsers from './pages/GetUsers';
import DeleteUser from './pages/DeleteUser';


const App = () => {
    return (
      <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register/>} />
        <Route path="/" element={<Login/>} />
        <Route path="/afterLogin" element = {<AfterLogin/>} />
        <Route path="/sign_up_confirmation" element = {<SignUpConfirmation/>} />
        <Route path="/forgot_password" element={<ForgotPassword/>} />
        <Route path="/waiting_to_reset_password" element={<WaitingToResetPassword/>} />
        <Route path="/reset_password/:token" element={<ResetPassword/>} />
        <Route path="/edit_your_profile" element={<EditProfile/>} />
        <Route path="/profile" element={<Profile/>} />
        <Route path="/logout" element={<Logout/>} />
        <Route path="/profile/verify_current_password" element={<VerifyCurrentPassword/>} />
        <Route path="/profile/reset_current_password" element={<ResetCurrentPasseword/>} />
        <Route path="/profile/edit_profile" element={<EditProfile/>} />
        <Route path="/add_admin" element={<AddAdmin/>} />
        <Route path="/profile/get_users" element={<GetUsers/>} />
        <Route path="/profile/delete_user" element={<DeleteUser/>} />
      </Routes>
    </BrowserRouter>
    );
};

export default App;