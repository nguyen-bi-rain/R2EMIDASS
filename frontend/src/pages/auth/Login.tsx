import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginRequestSchema } from '../../utils/validations/authValidation';
import { LoginRequest } from '../../models/authType';
import authService from '../../services/auth.service';
import { useAuthContext } from '../../context/authContext';
import { toast } from 'react-toastify';
import { useState } from 'react';
import { decodeAndExtractUserId } from '../../utils/jwt-decode';
import { IoBookOutline } from 'react-icons/io5';
import { CgSpinnerAlt } from "react-icons/cg";

const Login = () => {
  const { setIsAuthenticated } = useAuthContext();
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequest>({
    resolver: zodResolver(loginRequestSchema),
  });

  const onSubmit = async (data: LoginRequest) => {
    setIsLoading(true);
    try {
      console.log(data);

      const res = await authService.login(data);
      const accessToken = res.data.accessToken;
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem("refreshToken", res.data.refreshToken);
      const decodedToken = decodeAndExtractUserId(accessToken);

      setIsAuthenticated(true);
      toast.success('Login successful!');
      if (decodedToken && decodedToken.role === "SUPER_USER") {
        window.location.href = "/books";
      } else {
        window.location.href = "/"; // Redirect to the dashboard or home page
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Login failed. Please try again.');
      console.error("Error during login:", error);
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
            <IoBookOutline className='ư-10 h-10'/>
            </div>
          </div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Library Portal</h1>
          <p className="text-gray-600">Sign in to access the management system</p>
        </div>

        <div className="bg-white rounded-xl shadow-lg overflow-hidden">
          <div className="p-8">
            <form onSubmit={handleSubmit(onSubmit)}>
              <div className="mb-6">
                <label htmlFor="userName" className="block text-sm font-medium text-gray-700 mb-1">
                  Username
                </label>
                <div className="relative">
                  <input
                    type="text"
                    id="userName"
                    {...register('userName')}
                    className={`block w-full pl-3 pr-3 py-3 rounded-lg border ${errors.userName ? 'border-red-500' : 'border-gray-300'
                      } focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
                    placeholder="Enter your username"
                  />
                </div>
                {errors.userName && (
                  <p className="mt-1 text-sm text-red-600">{errors.userName.message}</p>
                )}
              </div>

              <div className="mb-6">
                <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
                  Password
                </label>
                <div className="relative">
                  <input
                    type={showPassword ? 'text' : 'password'}
                    id="password"
                    {...register('password')}
                    className={`block w-full pl-3 pr-10 py-3 rounded-lg border ${errors.password ? 'border-red-500' : 'border-gray-300'
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

              <button
                type="submit"
                disabled={isLoading}
                className={`w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-white font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 ${isLoading ? 'bg-blue-400' : 'bg-blue-600 hover:bg-blue-700'
                  }`}
              >
                {isLoading ? (
                  <>
                    <CgSpinnerAlt className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" />
                    Signing in...
                  </>
                ) : (
                  'Sign in'
                )}
              </button>
            </form>
          </div>

          <div className="bg-gray-50 px-8 py-4 border-t border-gray-200">
            <p className="text-sm text-gray-600 text-center">
              Don't have an account?{' '}
              <a href="/register" className="font-medium text-blue-600 hover:text-blue-500">
                Register
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;