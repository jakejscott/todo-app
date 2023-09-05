import { zodResolver } from "@hookform/resolvers/zod";
import type { ActionArgs, V2_MetaFunction } from "@remix-run/node";
import { json, redirect } from "@remix-run/node";
import {
  Link,
  useActionData,
  useNavigation,
  useSubmit,
} from "@remix-run/react";
import { Loader2 } from "lucide-react";
import { useEffect, useRef } from "react";
import { useForm } from "react-hook-form";
import invariant from "tiny-invariant";
import * as z from "zod";
import { Button } from "~/components/ui/button";
import { Checkbox } from "~/components/ui/checkbox";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "~/components/ui/form";
import { Input } from "~/components/ui/input";
import { Separator } from "~/components/ui/separator";
import type { CreateTodoItemRequest } from "~/lib/api.server";
import { createTodo } from "~/lib/api.server";

const FormSchema = z.object({
  description: z.string().min(2).max(50),
  isCompleted: z.boolean(),
});

export type FieldErrors = {
  description?: string[] | undefined;
  isCompleted?: string[] | undefined;
};

export const meta: V2_MetaFunction = () => {
  return [
    { title: "Todo App - New" },
    { name: "description", content: "Edit todo" },
  ];
};

export const action = async ({ request, params }: ActionArgs) => {
  const form = await request.formData();

  const description = form.get("description") as string;
  const isCompleted = form.get("isCompleted") == "on" ? true : false;
  invariant(description, "description is required");

  const createRequest: CreateTodoItemRequest = {
    description: description,
    isCompleted: isCompleted,
  };

  const parsed = FormSchema.safeParse(createRequest);
  if (!parsed.success) {
    const fieldErrors: FieldErrors = parsed.error.formErrors.fieldErrors;
    return json({ errors: fieldErrors });
  }

  // Handle any server side errors
  const { problem } = await createTodo(createRequest);
  if (problem) {
    const fieldErrors: FieldErrors = {};
    fieldErrors.description = problem.errors["Description"];
    fieldErrors.isCompleted = problem.errors["IsCompleted"];
    return json({ errors: fieldErrors });
  }

  return redirect("/");
};

export default function Index() {
  const actionData = useActionData<typeof action>();
  const submit = useSubmit();
  const navigation = useNavigation();
  const formRef = useRef<HTMLFormElement>(null);

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      description: "",
      isCompleted: false,
    },
  });

  useEffect(() => {
    if (actionData?.errors) {
      if (actionData.errors.description) {
        for (const error of actionData.errors.description) {
          form.setError("description", { message: error });
        }
      }

      if (actionData.errors.isCompleted) {
        for (const error of actionData.errors.isCompleted) {
          form.setError("description", { message: error });
        }
      }
    }
  }, [actionData?.errors, form]);

  async function onSubmit(values: z.infer<typeof FormSchema>) {
    const formData = new FormData(formRef.current!);
    submit(formData, { action: ".", method: "post", replace: true });
  }

  return (
    <div className="container">
      <div className="mt-10 overflow-hidden rounded-[0.5rem] border bg-background shadow">
        <div className="p-10">
          <div className="flex items-center justify-between space-y-2">
            <div>
              <h2 className="text-2xl font-bold tracking-tight">
                Todos App 🪅
              </h2>
              <p className="text-muted-foreground">
                Create your Todo by setting the description and completed
                status.
              </p>
            </div>
            <div className="flex items-center space-x-2">
              <Button asChild variant="link">
                <Link to={"/"}>Back</Link>
              </Button>
            </div>
          </div>

          <div className="mt-6 space-y-6">
            <Separator />

            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-8"
                ref={formRef}
              >
                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Description</FormLabel>
                      <FormControl>
                        <Input placeholder="Get some milk" {...field} />
                      </FormControl>
                      <FormDescription>
                        This is your todo description.
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="isCompleted"
                  render={({ field }) => (
                    <FormItem>
                      <div className="items-top flex space-x-2">
                        <Checkbox
                          id="isCompleted"
                          name="isCompleted"
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                        <div className="grid gap-1.5 leading-none">
                          <label
                            htmlFor="isCompleted"
                            className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                          >
                            Completed
                          </label>
                          <p className="text-sm text-muted-foreground">
                            Tick the box if the item is complete.
                          </p>
                        </div>
                      </div>
                    </FormItem>
                  )}
                />
                <Button type="submit" disabled={navigation.state != "idle"}>
                  {navigation.state != "idle" && (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  )}
                  Submit
                </Button>
              </form>
            </Form>
          </div>
        </div>
      </div>
    </div>
  );
}
