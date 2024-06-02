import { Modal, Box, Typography } from "@mui/material";

const modalPositionStyle = {
  height: "100%",
  outline: 0,
  display: "flex",
  WebkitBoxPack: "center",
  justifyContent: "center",
  WebkitBoxAlign: "center",
  alignItems: "center",
  opacity: 1,
  transition: "opacity 225ms cubic-bezier(0.4, 0, 0.2, 1) 0ms",
};

const insideStyle = {
  transition: "box-shadow 300ms cubic-bezier(0.4, 0, 0.2, 1) 0ms",
  borderRadius: 2,
  boxShadow: "rgba(0, 0, 0, 0.08) 0px 9px 46px",
  margin: "32px",
  position: "relative",
  overflowY: "auto",
  display: "flex",
  flexDirection: "column",
  maxHeight: "calc(100% - 64px)",
  maxWidth: "600px",
  backgroundImage: "none",
  backgroundColor: "rgb(255, 255, 255)",
  width: "calc(100% - 64px)",
  p: 5,
};

export interface CustomModalProps {
  open: boolean;
  handleClose: () => void;
  title: string;
  children: React.ReactNode;
  sxProps?: object;
}

export const CustomModal = ({
  open,
  handleClose,
  title,
  children,
  sxProps,
}: CustomModalProps) => {
  return (
    <Modal
      open={open}
      sx={modalPositionStyle}
      onClose={handleClose}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
    >
      <Box
        sx={{
          ...insideStyle,
          ...(sxProps || {}),
        }}
      >
        <Box
          sx={{
            backgroundColor: "background.paper",
            flex: "1 1 auto",
            alignItems: "center",
            display: "flex",
            justifyContent: "center",
          }}
        >
          <Box
            sx={{
              maxWidth: 550,
              px: 3,
              width: "100%",
            }}
          >
            <div>
              <Typography
                id="modal-modal-title"
                sx={{ my: 3 }}
                variant="h6"
                component="h2"
              >
                {title}
              </Typography>
              {children}
            </div>
          </Box>
        </Box>
      </Box>
    </Modal>
  );
};
