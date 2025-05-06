import { useState } from 'react'
import { useAuthContext } from '../../context/authContext'
import authService from '../../services/auth.service'

export const Header = () => {
  const { role } = useAuthContext()
  const path = window.location.pathname.split('/')[1]
  const [isDropdownOpen, setIsDropdownOpen] = useState(false)

  const handleLogout = () => {
    authService.logout()
    window.location.reload()
  }

  const toggleDropdown = () => {
    setIsDropdownOpen(!isDropdownOpen)
  }

  return (
    <header className="bg-white shadow-sm p-4 flex justify-between items-center">
      <h2 className="text-xl font-semibold text-gray-800 capitalize">{path}</h2>
      <div className="relative flex items-center space-x-4">
        <div className="flex items-center space-x-2" />
        <img
          src="https://randomuser.me/api/portraits/men/16.jpg"
          alt="User"
          className="h-8 w-8 rounded-full cursor-pointer"
          onClick={toggleDropdown}
        />
        <span className="text-gray-700 hidden md:block">{role}</span>
        {isDropdownOpen && (
          <div className="  absolute right-20 top-8 mt-2 w-30 bg-white border border-gray-200 rounded shadow-lg">
            <button
            
              onClick={handleLogout}
              className="hover:cursor-pointer block w-full text-center px-4 py-2 text-gray-700 hover:bg-gray-100"
            >
              Logout
            </button>
          </div>
        )}
      </div>
    </header>
  )
}
