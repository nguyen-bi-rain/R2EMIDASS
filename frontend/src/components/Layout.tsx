import { Outlet } from 'react-router-dom'
import { Header } from './shared/Header'
import Navbar from './shared/Navbar'

const AdminLayout = () => {
  return (
    <div className='flex h-screen overflow-hidden'>
      <Navbar />
      <div className='flex-1 overflow-auto'>
        <Header />
        <div className='p-4 bg-gray-200 '>
        <Outlet />
        </div>
      </div>

    </div>
  )
}

export default AdminLayout