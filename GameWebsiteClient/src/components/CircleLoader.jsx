import React from "react";
import { motion } from "framer-motion";


const circleStyle = {
  display: "block",
  width: "2rem",
  height: "2rem",
  border: "0.5rem solid #e9e9e9",
  borderTop: "0.5rem solid #3498db",
  borderRadius: "50%",
  position: "absolute",
  boxSizing: "border-box",
  top: "8px",
  right: "8px"
};

const spinTransition = {
  repeat: Infinity,
  duration: 1,
  ease: "linear"
};

export default function CircleLoader() {
  return (
    <div className="loaderContainer centerScreen">
      <motion.span
        style={circleStyle}
        animate={{ rotate: 360 }}
        transition={spinTransition}
      />
    </div>
  );
}