import { useState, useRef, useEffect } from 'react';
import { IoIosArrowDown } from 'react-icons/io';

interface SelectOption {
    value: string | number;
    label: string;
}

interface CustomSelectProps {
    options: SelectOption[];
    value?: string | number;
    onChange?: (value: string | number) => void;
    placeholder?: string;
    className?: string;
    disabled?: boolean;
}

export const CustomSelect = ({
    options,
    value,
    onChange,
    placeholder = 'Select...',
    className = '',
    disabled = false,
}: CustomSelectProps) => {
    const [isOpen, setIsOpen] = useState(false);
    const [selectedLabel, setSelectedLabel] = useState('');
    const selectRef = useRef<HTMLDivElement>(null);

    // Set initial selected label
    useEffect(() => {
        if (value) {
            const selectedOption = options.find(option => option.value === value);
            setSelectedLabel(selectedOption?.label || '');
        } else {
            setSelectedLabel('');
        }
    }, [value, options]);

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (selectRef.current && !selectRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const handleSelect = (option: SelectOption) => {
        setSelectedLabel(option.label);
        onChange?.(option.value);
        setIsOpen(false);
    };

    return (
        <div
            ref={selectRef}
            className={`relative ${className}`}
        >
            <button
                type="button"
                className={`w-full flex items-center justify-between px-4 py-2 text-left border rounded-lg transition-all duration-200
          ${isOpen ? 'border-blue-500 ring-2 ring-blue-200' : 'border-gray-300 hover:border-gray-400'}
          ${disabled ? 'bg-gray-100 cursor-not-allowed opacity-70' : 'bg-white cursor-pointer'}
        `}
                onClick={() => !disabled && setIsOpen(!isOpen)}
                disabled={disabled}
                aria-haspopup="listbox"
                aria-expanded={isOpen}
            >
                <span className={`truncate ${!selectedLabel ? 'text-gray-400' : 'text-gray-800'}`}>
                    {selectedLabel || placeholder}
                </span>
                <IoIosArrowDown className={` w-5 h-5 text-gray-400 transition-transform duration-200 ${isOpen ? 'transform rotate-180' : ''}`} />
            </button>

            {isOpen && (
                <select
                    value={value}
                    className="absolute z-10 w-full mt-1 bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 py-1 focus:outline-none"
                    size={options.length}
                    onChange={(e) => {
                        const selectedOption = options.find(option => option.value.toString() === e.target.value);
                        if (selectedOption) handleSelect(selectedOption);
                    }}
                >
                    {options.map((option) => (
                        <option
                            key={option.value}
                            value={option.value}
                            className={`px-4 py-2 cursor-pointer transition-colors duration-150
                ${value === option.value ? 'bg-blue-50 text-blue-600' : 'hover:bg-gray-50 text-gray-800'}
              `}
                        >
                            {option.label}
                        </option>
                    ))}
                </select>
            )}
        </div>
    );
};