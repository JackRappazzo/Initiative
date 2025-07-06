import React, { useState, useCallback } from 'react';

interface NumericInputProps {
  /** The current value */
  value: number | undefined;
  /** Optional maximum value */
  max?: number;
  /** Optional minimum value */
  min?: number;
  /** Callback when the value changes */
  onChange: (newValue: number) => void;
  /** Optional callback when the input loses focus */
  onBlur?: () => void;
  /** Optional aria-label for accessibility */
  ariaLabel?: string;
  /** Optional CSS class name */
  className?: string;
  /** Optional placeholder text */
  placeholder?: string;
}

/**
 * A numeric input component that supports direct entry and arithmetic operations (+/-).
 * The input accepts regular numbers and arithmetic operations (e.g., +5, -3).
 * Values are automatically clamped between min and max if provided.
 */
export const NumericInput: React.FC<NumericInputProps> = ({
  value,
  max,
  min,
  onChange,
  onBlur,
  ariaLabel,
  className,
  placeholder
}) => {
  const [inputValue, setInputValue] = useState(() => 
    value !== undefined ? value.toString() : '0'
  );

  const calculateNewValue = useCallback((input: string): number => {
    // Step 1: Clean and validate input
    input = input.trim();
    if (!input || !/^[+-]?\d+$/.test(input)) 
      return 0;
    
    // Step 2: Check for arithmetic operators
    const shouldDoMath = input.startsWith('+') || (input.startsWith('-') && value !== undefined);
    
    // Step 3: Direct number input
    if (!shouldDoMath) 
      return parseInt(input);
    
    // Step 4: Arithmetic
    const currentValue = value ?? 0;
    const operator = input[0];
    const operand = parseInt(input.substring(1));
    
    return operator === '+' 
      ? currentValue + operand 
      : currentValue - operand;

  }, [value]);

  const clampValue = useCallback((val: number): number => {
    if (max !== undefined && val > max)
      return max;
    if (min !== undefined && val < min)
      return min;
    return val;
  }, [max, min]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newInput = e.target.value;
    setInputValue(newInput);
  };

  const evaluateAndUpdate = useCallback(() => {
    const newValue = calculateNewValue(inputValue);
    if (!isNaN(newValue)) {
      const clampedValue = clampValue(newValue);
      onChange(clampedValue);
      setInputValue(clampedValue.toString());
    } else {
      setInputValue((value ?? 0).toString());
    }
  }, [calculateNewValue, clampValue, inputValue, onChange, value]);

  const handleBlur = () => {
    evaluateAndUpdate();
    onBlur?.();
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      evaluateAndUpdate();
      e.currentTarget.blur();
    }
  };

  const handleFocus = (e: React.FocusEvent<HTMLInputElement>) => {
    e.currentTarget.select();
  };

  return (
    <input
      type="text"
      value={inputValue}
      onChange={handleChange}
      onBlur={handleBlur}
      onFocus={handleFocus}
      onKeyDown={handleKeyDown}
      aria-label={ariaLabel}
      className={className}
      placeholder={placeholder}
    />
  );
};
