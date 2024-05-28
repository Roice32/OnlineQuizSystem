import { Snackbar } from "@mui/material";
import MuiAlert from "@mui/material/Alert";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { closeSnackbar } from "../redux/Snackbar/SnackbarState";

export default function GeneralSnackbar() {
  const snackbarState = useSelector((state: RootState) => state.snackbar);
  const { open, data } = snackbarState;
  const dispatch = useDispatch();
  const onClose = () => {
    dispatch(closeSnackbar());
  };
  return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      onClose={onClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
    >
      <MuiAlert
        elevation={6}
        variant="filled"
        onClose={onClose}
        severity={data?.severity || "success"}
      >
        {data?.message}
      </MuiAlert>
    </Snackbar>
  );
}
