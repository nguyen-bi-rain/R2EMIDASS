import React from 'react';
import { FieldError } from 'react-hook-form';

interface InputProps {
    id: string;
    label: string;
    type: string;
    placeholder: string;
    register: any;
    error?: FieldError;
    className?: string;
    icon?: React.ReactNode;
}

const Input: React.FC<InputProps> = ({ icon, id, label, type, placeholder, register, error, className, ...props }) => {
    return (
        <div>
            <label htmlFor={id} className="block text-sm font-medium text-gray-700">
                {label}
            </label>
            <div className="relative mt-1">
                {icon && (
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        {icon}
                    </div>
                )}
                <input
                    defaultValue={props.defaultValue}
                    {...props}
                    id={id}
                    type={type}
                    {...register(id)}
                    className={`${className} ${icon ? 'pl-10' : ''} w-full px-3 py-2 border ${error ? 'border-red-500' : 'border-gray-300'
                        } rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500`}
                    placeholder={placeholder}
                />
            </div>
            {error && <p className="text-red-500 text-sm mt-1">{error.message}</p>}
        </div>
    );
};

export default Input;