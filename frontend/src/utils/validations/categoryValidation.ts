import { z } from "zod";
import { CategoryType } from "../../models/categoryType";

export const categoryRequestSchema = z.object({
    name: z.string()
        .min(3, "Name must be at least 5 characters long")
        .max(50, "Name must be at most 200 characters long"),
    description: z.string()
});

export const validateCategoryRequest = (data: CategoryType) => {
    return categoryRequestSchema.safeParse(data);
}