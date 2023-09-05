import type { LoaderArgs, V2_MetaFunction } from "@remix-run/node";
import { Link, useLoaderData, useNavigate } from "@remix-run/react";
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
import { ChevronLeft, ChevronRight } from "lucide-react";

export const meta: V2_MetaFunction = () => {
  return [
    { title: "Todo App" },
    { name: "description", content: "There's always stuff todo" },
  ];
};

export const loader = async ({ request }: LoaderArgs) => {
  const url = new URL(request.url);

  let pageSize: number | undefined = 10;
  let currentPageToken = url.searchParams.get("nextPageToken");

  const { items, nextPageToken } = await listTodos({
    pageSize: pageSize,
    nextPageToken: currentPageToken,
  });

  let showPrevPage = currentPageToken ? true : false;
  let showNextPage = false;
  let nextPageUrl = `/?pageSize=${pageSize}`;

  if (nextPageToken && items.length >= pageSize) {
    nextPageUrl += `&nextPageToken=${nextPageToken}`;
    showNextPage = true;
  }

  return {
    todos: items,
    nextPageUrl,
    showNextPage,
    showPrevPage,
  };
};

export default function Index() {
  const data = useLoaderData<typeof loader>();
  const navigate = useNavigate();

  return (
    <div className="container">
      <div className="mt-10 overflow-hidden rounded-[0.5rem] border bg-background shadow">
        <div className="p-10">
          <div className="flex items-center justify-between space-y-2">
            <div>
              <h2 className="text-2xl font-bold tracking-tight"> Todos 🪅</h2>
              <p className="text-muted-foreground">
                Here's a list of your tasks for this month!
              </p>
            </div>
            <div className="flex items-center space-x-2">
              <Button asChild variant="ghost">
                <Link to={"/todos/new "}>Create</Link>
              </Button>
            </div>
          </div>

          <Table className="mt-5">
            <TableCaption>A list of your recent todos.</TableCaption>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px]">Id</TableHead>
                <TableHead>Description</TableHead>
                <TableHead className="">Completed</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.todos.map((item) => (
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

          <div className="text-right space-x-4">
            {data.showPrevPage && (
              <Button
                variant={"secondary"}
                onClick={() => {
                  navigate(-1);
                }}
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>
            )}

            {data.showNextPage && (
              <Button asChild variant="secondary">
                <Link to={data.nextPageUrl}>
                  <ChevronRight className="h-4 w-4" />
                </Link>
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
