import axiosInstance from "../api/axiosInstance";
import { ENDPOINS } from "../constants";
import { BookBorrowType, BookType } from "../models/bookType";

class bookService {
    async getAllBooks(pageIndex: number = 1, pageSize: number = 10, searchTerm: string | null = "", categoryId: string | null = null) {
        try {
            const response = await axiosInstance.get(ENDPOINS.book.getAll, {
                params: {
                    pageSize: pageSize,
                    pageIndex: pageIndex,
                    searchTerm: searchTerm,
                    categoryId: categoryId
                }
            });
            return response.data;
        } catch (error) {
            console.error("Get all books error:", error);
            throw error;
        }
    }
    async getBookById(id: string) {

        try {
            const response = await axiosInstance.get(ENDPOINS.book.getById(id));
            return response.data;
        } catch (error) {
            console.error("Get book by ID error:", error);
            throw error;
        }
    }
    async createBook(data: BookType) {

        try {
            const response = await axiosInstance.post(ENDPOINS.book.create, data);
            return response.data;
        } catch (error) {
            console.error("Create book error:", error);
            throw error;
        }
    }
    async updateBook(id: string, data: any) {

        try {
            const response = await axiosInstance.put(ENDPOINS.book.update(id), data);
            return response.data;
        } catch (error) {
            console.error("Update book error:", error);
            throw error;
        }
    }
    async deleteBook(id: string) {

        try {
            const response = await axiosInstance.delete(ENDPOINS.book.delete(id));
            return response.data;
        } catch (error) {
            console.error("Delete book error:", error);
            throw error;
        }
    }
    async borrowBook(data: BookBorrowType) {
        try {
            const response = await axiosInstance.post(ENDPOINS.book.borrow, data);
            return response.data;
        }
        catch (error) {
            console.error("Borrow book error:", error);
            throw error;
        }
    }
    async getAllBorrowedBooksRequest(pageIndex: number = 1, pageSize: number = 10, status: number | null = -1) {
        try {
            const response = await axiosInstance.get(ENDPOINS.book.getAllBorrowedBooksRequest, {
                params: {
                    pageSize: pageSize,
                    pageIndex: pageIndex,
                    status: status
                }
            });
            return response.data;
        } catch (error) {
            console.error("Get all borrowed books request error:", error);
            throw error;
        }
    }
    async getAllBorrowedBooksRequestByUserId(userId: string | null, pageIndex: number = 1, pageSize: number = 10, status: number | null = -1) {
        try {
            const response = await axiosInstance.get(ENDPOINS.book.getAllBorrowedBooksByUser(userId), {
                params: {
                    pageSize: pageSize,
                    pageIndex: pageIndex,
                    status: status
                }
            });
            return response.data;
        } catch (error) {
            console.error("Get all borrowed books request error:", error);
            throw error;
        }
    }
    async updateBorrowedBookRequest( data: any) {

        try {
            const response = await axiosInstance.put(ENDPOINS.book.updateBorrowedBookRequest, data);
            return response.data;
        } catch (error) {
            console.error("Update borrowed book request error:", error);
            throw error;
        }
    }
    async getCurrentMonthRequestCount(userId: string | null) {
        try {
            const response = await axiosInstance.get(ENDPOINS.book.getRequestCountMonthLy(userId));
            return response.data;
        }
        catch (error) {
            console.error("Get current month request count error:", error);
            throw error;
        }
    }
}

export default new bookService();