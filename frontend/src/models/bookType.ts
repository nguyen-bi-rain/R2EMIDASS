export interface BookType {
    title: string;
    author: string;
    description: string;
    quantity: number;
    available: number;
    publishedDate: string;
    categoryId: number;
}

interface bookBorrowingRequestDetails {
    bookBorrowingRequestId: number;
    bookId: string;
    createdAt: Date;

}
export interface BookBorrowType {
    requestorId: string;
    bookBorrowingRequestDetails: bookBorrowingRequestDetails[];

}

export interface BookBorrowResponseType {
    id: number;
    requestName: string;
    approveName: string;
    dateRequest: string;
    status: number;
    dateExpired: string;
    updatedAt: string;
    bookBorrowingRequestDetails: {
        bookBorrowingRequestId: number;
        bookId: string;
        book: {
            id: string;
            title: string;
            author: string;
            description: string;
            quantity: number;
            available: number;
            publishedDate: string;
            categoryId: number;
        };
    }[];
}

export interface Book {
    id: string;
    title: string;
    author: string;
    description: string;
    quantity: number;
    available: number;
    publishedDate: string;
    categoryName: string;
}