import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { registerRequestSchema } from '../../utils/validations/authValidation';
import { RegisterRequest } from '../../models/authType';
import authService from '../../services/auth.service';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { toast } from 'react-toastify';
import Input from '../../components/ui/Input';
import { FiMail, FiUser, FiPhone, FiLock } from 'react-icons/fi';
import { IoBookOutline } from 'react-icons/io5';
import { CgSpinnerAlt } from 'react-icons/cg';

const Register = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const navigate = useNavigate();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<RegisterRequest>({
        resolver: zodResolver(registerRequestSchema),
    });

    const onSubmit = async (data: RegisterRequest) => {
        setIsLoading(true);
        try {
            // Create a new object without the confirmPassword field
            const { confirmPassword, ...registrationData } = data;

            // Convert gender string to number
            registrationData.gender = Number(registrationData.gender);

            console.log(registrationData);

            await authService.register(registrationData);
            toast.success('Registration successful! Please login.');
            navigate('/login');
        } catch (error: any) {
            toast.error(error.response?.data?.message || 'Registration failed. Please try again.');
            console.error('Error during registration:', error);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
            <div className="w-full max-w-md">
                <div className="text-center mb-8">
                    <div className="flex justify-center mb-4">
                        <div className="bg-blue-600 text-white p-3 rounded-lg">
                        <IoBookOutline className='w-10 h-10'/>
                        </div>
                    </div>
                    <h1 className="text-3xl font-bold text-gray-800 mb-2">Library Portal</h1>
                    <p className="text-gray-600">Create your account</p>
                </div>

                <div className="bg-white rounded-xl shadow-lg overflow-hidden">
                    <div className="p-8">
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <div className="mb-6">
                                <Input
                                    id="email"
                                    label="Email"
                                    type="email"
                                    placeholder="Enter your email"
                                    register={register}
                                    error={errors.email}
                                    icon={<FiMail className="text-gray-400" />}
                                />
                            </div>

                            <div className="mb-6">
                                <Input
                                    id="userName"
                                    label="Username"
                                    type="text"
                                    placeholder="Enter your username"
                                    register={register}
                                    error={errors.userName}
                                    icon={<FiUser className="text-gray-400" />}
                                />
                            </div>

                            <div className="mb-6">
                                <Input
                                    id="phoneNumber"
                                    label="Phone Number"
                                    type="text"
                                    placeholder="Enter your phone number"
                                    register={register}
                                    error={errors.phoneNumber}
                                    icon={<FiPhone className="text-gray-400" />}
                                />
                            </div>

                            <div className="mb-6">
                                <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
                                    Password
                                </label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                        <FiLock className="text-gray-400" />
                                    </div>
                                    <input
                                        type={showPassword ? 'text' : 'password'}
                                        id="password"
                                        {...register('password')}
                                        className={`block w-full pl-10 pr-10 py-3 rounded-lg border ${
                                            errors.password ? 'border-red-500' : 'border-gray-300'
                                        } focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
                                        placeholder="Enter your password"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="absolute inset-y-0 right-0 pr-3 flex items-center"
                                    >
                                        {showPassword ? 'Hide' : 'Show'}
                                    </button>
                                </div>
                                {errors.password && (
                                    <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>
                                )}
                            </div>

                            <div className="mb-6">
                                <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700 mb-1">
                                    Confirm Password
                                </label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                        <FiLock className="text-gray-400" />
                                    </div>
                                    <input
                                        type={showConfirmPassword ? 'text' : 'password'}
                                        id="confirmPassword"
                                        {...register('confirmPassword')}
                                        className={`block w-full pl-10 pr-10 py-3 rounded-lg border ${
                                            errors.confirmPassword ? 'border-red-500' : 'border-gray-300'
                                        } focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
                                        placeholder="Confirm your password"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                        className="absolute inset-y-0 right-0 pr-3 flex items-center"
                                    >
                                        {showConfirmPassword ? 'Hide' : 'Show'}
                                    </button>
                                </div>
                                {errors.confirmPassword && (
                                    <p className="mt-1 text-sm text-red-600">{errors.confirmPassword.message}</p>
                                )}
                            </div>

                            <div className="mb-6">
                                <label htmlFor="gender" className="block text-sm font-medium text-gray-700 mb-1">
                                    Gender
                                </label>
                                <select
                                    id="gender"
                                    {...register("gender")}
                                    className={`mt-1 block w-full px-3 py-3 rounded-lg border ${errors.gender ? 'border-red-500' : 'border-gray-300'} focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
                                >
                                    <option value="0">Male</option>
                                    <option value="1">Female</option>
                                    <option value="2">Other</option>
                                </select>
                                {errors.gender && (
                                    <p className="mt-1 text-sm text-red-600">{errors.gender.message}</p>
                                )}
                            </div>

                            <button
                                type="submit"
                                disabled={isLoading}
                                className={`w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-white font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 ${isLoading ? 'bg-blue-400' : 'bg-blue-600 hover:bg-blue-700'}`}
                            >
                                {isLoading ? (
                                    <>
                                        <CgSpinnerAlt className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" />
                                        Registering...
                                    </>
                                ) : (
                                    'Register'
                                )}
                            </button>
                        </form>
                    </div>

                    <div className="bg-gray-50 px-8 py-4 border-t border-gray-200">
                        <p className="text-sm text-gray-600 text-center">
                            Already have an account?{' '}
                            <button 
                                onClick={() => navigate('/login')} 
                                className="font-medium text-blue-600 hover:text-blue-500"
                            >
                                Sign in
                            </button>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Register;
