import axiosInstance from "../api/axiosInstance";
import { ENDPOINS } from "../constants";
import { CategoryType } from "../models/categoryType";


class categoryService {
    async getAllCategories(page: number, itemsPerPage: number) {

        try {
            const response = await axiosInstance.get(`${ENDPOINS.category.getAll}`,
                {
                    params :{
                        pageIndex: page,
                        pageSize: itemsPerPage
                    }
                    
                }
            );
            return response.data;
        } catch (error) {
            console.error("Get all categories error:", error);
            throw error;
        }
    }
    async getCategoryById(id: string) {

        try {
            const response = await axiosInstance.get(ENDPOINS.category.getById(id));
            return response.data;
        } catch (error) {
            console.error("Get category by ID error:", error);
            throw error;
        }
    }
    async createCategory(data: CategoryType) {

        try {
            const response = await axiosInstance.post(ENDPOINS.category.create, data);
            return response.data;
        } catch (error) {
            console.error("Create category error:", error);
            throw error;
        }
    }
    async updateCategory(id: string, data: object) {

        try {
            const response = await axiosInstance.put(ENDPOINS.category.update, { id, ...data });
            return response.data;
        } catch (error) {
            console.error("Update category error:", error);
            throw error;
        }
    }

    async deleteCategory(id: string) {

        try {
            const response = await axiosInstance.delete(ENDPOINS.category.delete(id));
            return response.data;
        } catch (error) {
            console.error("Delete category error:", error);
            throw error;
        }
    }
}

export default new categoryService();   