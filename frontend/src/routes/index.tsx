import { useRoutes } from 'react-router-dom'
import Layout from '../components/Layout'
import { JSX } from 'react'
import Login from '../pages/auth/Login'
import Register from '../pages/auth/Register'
import { useAuthContext } from '../context/authContext'
import { AccessDenied } from '../pages/AccessDenied'
import ListBook from '../pages/book/ListBook'
import Category from '../pages/category/Category'
import CreateCategory from '../components/Category/CreateCategory'
import EditCategory from '../components/Category/EditCategory'
import CreateBook from '../components/Book/CreateBook'
import EditBook from '../components/Book/EditBook'
import BorrowingRequests from '../pages/bookborrow/BookBorrowed'
import UserBookList from '../components/Book/UserBookList'

export const AppRoute = () => {
    const routes = useRoutes([
        { path: '/login', element: <Login /> },
        { path: '/register', element: <Register /> },
        {
            path: '/',
            element: <ProtectedRoute><Layout /></ProtectedRoute>,
            children: [
            { path: '/', element: <UserBookList /> },
                { path: '/books', element: <PermissionRoute><ListBook/></PermissionRoute> },
                { path: '/admin/create-book', element: <PermissionRoute><CreateBook/></PermissionRoute> },
                { path: '/admin/edit-book/:id', element: <PermissionRoute><EditBook/></PermissionRoute> },
                { path: '/categories', element: <PermissionRoute><Category/></PermissionRoute> },
                { path: '/admin/create-category', element: <PermissionRoute><CreateCategory/></PermissionRoute> },
                { path: '/admin/edit-category/:id', element: <PermissionRoute><EditCategory/></PermissionRoute> },
                {path: "admin/book-borrowed", element: <BorrowingRequests/>},
            ]
        }
    ])
    return routes
}


export function ProtectedRoute({ children }: { readonly children: JSX.Element }) {
    const { isAuthenticated } = useAuthContext()
    if (!isAuthenticated) {
        return <Login />
    }
    return children
}

export function PermissionRoute({ children }: { readonly children: JSX.Element }) {
    const { role } = useAuthContext()
    if (role !== "SUPER_USER") {
        return <AccessDenied />
    }
    return children
}
