export const NAVIGATION_ITEMS = (role: 'NORMAL_USER' | 'SUPER_USER') => {
    if (role === 'NORMAL_USER') {
        return [
            {
                name: 'Home',
                icon: <i className="fas fa-exchange-alt"></i>,
                path: '/',
                isActive: true,
            },
            {
                name: 'My Borrowed Books',
                icon: <i className="fas fa-book"></i>,
                path: '/admin/book-borrowed',
                isActive: false,
            },
        ];
    } else if (role === 'SUPER_USER') {
        return [
            {
                name: "Books",
                icon: <i className="fas fa-book"></i>,
                path: "/books",
                isActive: true,
            },
            {
                name: "Category",
                icon: <i className="fa-solid fa-layer-group"></i>,
                path: "/categories",
                isActive: false,
            },
            {
                name: "Book Borrowed",
                icon: <i className="fas fa-exchange-alt"></i>,
                path: "/admin/book-borrowed",
                isActive: false,
            }

        ]
    }
    return [];
};


export const ENDPOINS = {
    auth: {
        login: "/Auth/login",
        register: "/Auth/register",
    },
    category: {
        getAll: '/Category',
        getById: (id: string) => `/Category/${id}`,
        create: '/Category',
        update: `/Category`,
        delete: (id: string) => `/Category/${id}`,
    },
    book: {
        getAll: "/Book",
        getById: (id: string) => `/Book/${id}`,
        create: "/Book",
        update: (id: string) => `/Book/${id}`,
        delete: (id: string) => `/Book/${id}`,
        borrow: "/BorrowBook/create",
        getAllBorrowedBooksRequest : "/BorrowBook",
        getAllBorrowedBooksByUser: (userId: string | null) => `/BorrowBook/user/${userId}`,
        updateBorrowedBookRequest : "/BorrowBook",
        getRequestCountMonthLy: (userId: string | null) => `/BorrowBook/current-monthly-requests/${userId}`,
    }
}