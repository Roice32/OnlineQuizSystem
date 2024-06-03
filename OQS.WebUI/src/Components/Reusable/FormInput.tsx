import { useState } from "react";
const FormInput = (props) => {
  const [focused, setFocused] = useState(false);
  const { label, errorMessage, onChange, is, ...inputProps } = props;

  const handleFocus = (e) => {
    setFocused(true);
  };

  return (
    <div className="flex flex-col w-72">
      <label className="text-base text-[#0a2d2e]" >{label}</label>
      <input  className="p-2 my-3 rounded-md border border-[#6a8e8f]"
        {...inputProps}
        onChange={onChange}
        autoComplete={inputProps}
        onBlur={handleFocus}
        onFocus={() =>
          inputProps.name === "confirmPassword" && setFocused(true)
        }
        focused={focused.toString()}
      />
      <span className="text-sm text-red-600 p-1 hidden">{errorMessage}</span>
      <style>{`
        input:invalid[focused=true] {
        border: 1px solid red;
        }

        input:invalid[focused=true] ~ span {
        display: block;
          }
      `}</style>

    </div>
  );
};

export default FormInput;
