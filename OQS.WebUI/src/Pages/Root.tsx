import {Outlet} from "react-router-dom";
import GeneralSnackbar from "../Components/Snackbar";
import Navbar from "../Components/Navbar";
import {Theme} from "@radix-ui/themes";

export default function Root() {
    return (
        <>
            <Theme accentColor='teal'>
                <Navbar/>
                <GeneralSnackbar/>

                <Outlet/>
            </Theme>
        </>
    );
}
