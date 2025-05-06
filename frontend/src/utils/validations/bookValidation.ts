import { z } from "zod";

export const bookRequestSchema = z.object({
    title: z.string().min(1, "Title is required").max(100, "Title must be less than 100 characters"),
    author: z.string().min(1, "Author is required").max(50, "Author must be less than 50 characters"),
    description: z.string().min(1, "Description is required").max(500, "Description must be less than 500 characters"),
    quantity: z.string().min(1, "Quantity is required"),
    publishedDate: z.string().min(1, "Published date is required"),
});

export const validateBookRequest = (data: any) => {
    return bookRequestSchema.safeParse(data);
};