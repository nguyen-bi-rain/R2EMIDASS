import React from 'react';
import { FieldError } from 'react-hook-form';

interface TextAreaProps {
    id: string;
    label: string;
    placeholder: string;
    register: any;
    error?: FieldError;
    className?: string;
    row ?: number;
    col?: number;
}

const TextArea: React.FC<TextAreaProps> = ({ id, label, placeholder, register, error, className,row,col, ...props }) => {
    return (
        <div>
            <label htmlFor={id} className="block text-sm font-medium text-gray-700">
                {label}
            </label>
            <textarea
                {...props}
                id={id}
                {...register(id)}
                rows={row}
                cols={col}
                className={`${className} mt-1 w-full px-3 py-2 border ${error ? 'border-red-500' : 'border-gray-300'
                    } rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500`}
                placeholder={placeholder}
            />
            {error && <p className="text-red-500 text-sm mt-1">{error.message}</p>}
        </div>
    );
};

export default TextArea;