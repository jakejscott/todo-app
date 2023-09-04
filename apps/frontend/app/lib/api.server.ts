import { json } from "@remix-run/node";

export type ProblemDetails = {
  type: string;
  title: string;
  status: number;
  errors: {
    [key: string]: string[];
  };
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

export type CreateTodoItemRequest = {
  description: string;
  isCompleted: boolean;
};

export type UpdateTodoItemRequest = {
  description: string;
  isCompleted: boolean;
};

export type CreateTodoItemResponse = {
  item: DescribeTodoItemResponse;
};

const url: string = process.env.BACKEND_API!;

export async function listTodos(): Promise<ListTodoItemsResponse> {
  const httpResponse = await fetch(`${url}/v1/todos`, { method: "get" });
  if (httpResponse.status == 200) {
    const response = await httpResponse.json();
    return response as ListTodoItemsResponse;
  }

  throw json({ error: "Something went wrong" }, { status: 500 });
}

export async function describeTodo(
  id: string
): Promise<DescribeTodoItemResponse> {
  const httpResponse = await fetch(`${url}/v1/todos/${id}`, { method: "get" });

  if (httpResponse.status == 200) {
    const response = await httpResponse.json();
    return response as DescribeTodoItemResponse;
  }

  if (httpResponse.status == 404) {
    throw json({ error: "Todo not found" }, { status: 404 });
  }

  throw json({ error: "Something went wrong" }, { status: 500 });
}

export async function createTodo(
  request: CreateTodoItemRequest
): Promise<{ problem?: ProblemDetails; response?: CreateTodoItemResponse }> {
  const httpResponse = await fetch(`${url}/v1/todos`, {
    method: "post",
    body: JSON.stringify(request),
    headers: {
      "content-type": "application/json",
    },
  });

  const json = await httpResponse.json();

  if (httpResponse.status == 201) {
    var response = json as CreateTodoItemResponse;
    return { response };
  }

  if (httpResponse.status == 400) {
    const problem = json as ProblemDetails;
    return { problem };
  }

  throw json({ error: "Something went wrong" }, { status: 500 });
}

export async function updateTodo(
  id: string,
  request: UpdateTodoItemRequest
): Promise<{ problem?: ProblemDetails }> {
  const httpResponse = await fetch(`${url}/v1/todos/${id}`, {
    method: "put",
    body: JSON.stringify(request),
    headers: {
      "content-type": "application/json",
    },
  });

  if (httpResponse.status == 204) {
    return {};
  }

  if (httpResponse.status == 400) {
    const json = await httpResponse.json();
    const problem = json as ProblemDetails;
    return { problem };
  }

  if (httpResponse.status == 404) {
    throw json({ error: "Todo not found" }, { status: 404 });
  }

  throw json({ error: "Something went wrong" }, { status: 500 });
}

export async function deleteTodo(id: string): Promise<void> {
  const httpResponse = await fetch(`${url}/v1/todos/${id}`, {
    method: "delete",
  });

  if (httpResponse.status == 200) {
    return;
  }

  if (httpResponse.status == 404) {
    throw json({ error: "Todo not found" }, { status: 404 });
  }

  throw json({ error: "Something went wrong" }, { status: 500 });
}
