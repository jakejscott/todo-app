import type { V2_MetaFunction } from "@remix-run/node";
import { Link, useLoaderData } from "@remix-run/react";
import { Button } from "~/components/ui/button";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "~/components/ui/table";
import { listTodos } from "../lib/api.server";

export const meta: V2_MetaFunction = () => {
  return [
    { title: "Todo App" },
    { name: "description", content: "There's always stuff todo" },
  ];
};

export const loader = async () => {
  const todos = await listTodos();
  return { todos: todos };
};

export default function Index() {
  const data = useLoaderData<typeof loader>();

  return (
    <div className="container">
      <div className="mt-10 overflow-hidden rounded-[0.5rem] border bg-background shadow">
        <div className="p-10">
          <div className="flex items-center justify-between space-y-2">
            <div>
              <h2 className="text-2xl font-bold tracking-tight">Todos 🪅</h2>
              <p className="text-muted-foreground">
                Here's a list of your tasks for this month!
              </p>
            </div>
            <div className="flex items-center space-x-2">
              <Button asChild variant="link">
                <Link to={"/todos/new "}>Create</Link>
              </Button>
            </div>
          </div>

          <Table className="mt-5">
            <TableCaption>A list of your recent tasks.</TableCaption>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px]">Id</TableHead>
                <TableHead>Description</TableHead>
                <TableHead className="">Completed</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.todos.items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="">{item.id}</TableCell>
                  <TableCell className="">{item.description}</TableCell>
                  <TableCell className="">
                    {item.isCompleted ? <span>true</span> : <span>false</span>}
                  </TableCell>
                  <TableCell className="text-right">
                    <Link to={`/todos/${item.id}/edit`}>Edit</Link>
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
