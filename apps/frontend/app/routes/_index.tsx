import type { V2_MetaFunction } from "@remix-run/node";
import { useLoaderData } from "@remix-run/react";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "~/components/ui/table";

export const meta: V2_MetaFunction = () => {
  return [
    { title: "Todo App" },
    { name: "description", content: "There's always stuff todo" },
  ];
};

export type DescribeTodoItemResponse = {
  id: string;
  description: string;
  isCompleted: boolean;
};

export type ListTodoItemsResponse = {
  nextPageToken?: string;
  items: DescribeTodoItemResponse[];
};

export const loader = async () => {
  const url: string = process.env.BACKEND_API!;

  const httpResponse = await fetch(`${url}/v1/todos`);
  const response = (await httpResponse.json()) as ListTodoItemsResponse;

  return { todos: response };
};

export default function Index() {
  const data = useLoaderData<typeof loader>();

  return (
    <div className="container">
      <div className="mt-10 overflow-hidden rounded-[0.5rem] border bg-background shadow">
        <div className="p-10">
          <h2 className="text-2xl font-bold tracking-tight">Todos App 🪅</h2>
          <p className="text-muted-foreground">
            Here's a list of your tasks for this month!
          </p>

          <Table className="mt-5">
            <TableCaption>A list of your recent tasks.</TableCaption>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px]">Id</TableHead>
                <TableHead>Description</TableHead>
                <TableHead className="text-right">Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.todos.items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.id}</TableCell>
                  <TableCell>{item.description}</TableCell>
                  <TableCell className="text-right">
                    {item.isCompleted ? (
                      <span>Complete</span>
                    ) : (
                      <span>Not Complete</span>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>
    </div>
  );
}
