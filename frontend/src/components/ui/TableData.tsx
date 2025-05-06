import React, { useMemo, useState } from "react";
import { FaLongArrowAltDown,FaLongArrowAltUp } from "react-icons/fa";

type ColumnDefinition<T> = {
  key: keyof T;
  title: string;
  render?: (item: T) => React.ReactNode;
  sortable?: boolean;
  width?: string;
  align?: "left" | "center" | "right";
};

type SortConfig<T> = {
  key: keyof T | null;
  direction: "asc" | "desc";
};

type DataTableProps<T> = {
  columns: ColumnDefinition<T>[];
  data: T[];
  sortable?: boolean;
  onSort?: (key: keyof T, direction: "asc" | "desc") => void;
  striped?: boolean;
  hoverable?: boolean;
  bordered?: boolean;
  compact?: boolean;
  className?: string;
  rowKey?: (item: T, index: number) => string | number;
  emptyState?: React.ReactNode;
};

const DataTable = <T extends Record<string, any>>({
  columns,
  data,
  sortable = false,
  onSort = () => {},
  striped = false,
  hoverable = true,
  bordered = false,
  compact = false,
  className = "",
  rowKey = (item: T, index: number) => index,
  emptyState = (
    <div className="py-8 text-center text-gray-500">No data available</div>
  ),
}: DataTableProps<T>) => {
  const [sortConfig, setSortConfig] = useState<SortConfig<T>>({
    key: null,
    direction: "asc",
  });

  const handleSort = (key: keyof T) => {
    if (!sortable) return;

    let direction: "asc" | "desc" = "asc";
    if (sortConfig.key === key && sortConfig.direction === "asc") {
      direction = "desc";
    }
    setSortConfig({ key, direction });
    onSort(key, direction);
  };

  const sortedData = useMemo(() => {
    if (!sortable || !sortConfig.key) return data;

    return [...data].sort((a, b) => {
      if (a[sortConfig.key!] < b[sortConfig.key!]) {
        return sortConfig.direction === "asc" ? -1 : 1;
      }
      if (a[sortConfig.key!] > b[sortConfig.key!]) {
        return sortConfig.direction === "asc" ? 1 : -1;
      }
      return 0;
    });
  }, [data, sortConfig, sortable]);

  return (
    <div className="flex flex-col space-y-4">
      <div
        className={`overflow-x-auto rounded-lg border border-gray-200 shadow-sm ${className}`}
      >
        {sortedData.length === 0 ? (
          emptyState
        ) : (
          <table
            className={`w-full ${
              compact ? "text-sm" : "text-base"
            } ${bordered ? "border-separate border-spacing-0" : ""}`}
          >
            <thead>
              <tr>
                {columns.map((column) => (
                  <th
                    key={column.key as string}
                    className={`px-${compact ? 3 : 4} py-${
                      compact ? 2 : 3
                    } font-medium text-gray-700 bg-gray-100
                  ${
                    bordered
                      ? "border-b border-t first:border-l last:border-r border-gray-200 first:rounded-l-lg last:rounded-r-lg"
                      : ""
                  }
                  ${
                    (sortable && column.sortable !== false) || column.sortable
                      ? "cursor-pointer hover:bg-gray-200 transition-colors"
                      : ""
                  }
                  ${column.align === "center" ? "text-center" : "text-left"}`}
                    onClick={() =>
                      (column.sortable !== false && sortable) || column.sortable
                        ? handleSort(column.key)
                        : undefined
                    }
                    style={{ width: column.width }}
                  >
                    <div
                      className={`flex items-center ${
                        column.align === "center" ? "justify-center" : ""
                      }`}
                    >
                      {column.title}
                      {((sortable && column.sortable !== false) ||
                        column.sortable) &&
                        sortConfig.key === column.key && (
                          <span className="ml-2 text-gray-500">
                            {sortConfig.direction === "asc" ? (
                              <FaLongArrowAltDown className="h-4 w-4" />
                            ) : (
                              <FaLongArrowAltUp className="h-4 w-4" />
                            )}
                          </span>
                        )}
                    </div>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {sortedData.map((row, rowIndex) => (
                <tr
                  key={rowKey(row, rowIndex)}
                  className={`${hoverable ? "hover:bg-gray-50" : ""} ${
                    striped && rowIndex % 2 === 0 ? "bg-gray-50" : "bg-white"
                  }`}
                >
                  {columns.map((column) => (
                    <td
                      key={`${column.key as string}-${rowIndex}`}
                      className={`px-${compact ? 3 : 4} py-${
                        compact ? 2 : 3
                      } text-gray-700 ${
                        bordered
                          ? "border-b first:border-l last:border-r border-gray-200"
                          : ""
                      }`}
                    >
                      {column.render ? column.render(row) : row[column.key]}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default DataTable;