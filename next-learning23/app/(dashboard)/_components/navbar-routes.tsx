"use client";

import { UserButton, useAuth } from "@clerk/nextjs";
import { usePathname } from "next/navigation";
import { isTeacher } from "@/lib/teacher";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { LogOut } from "lucide-react";
import { ModeToggle } from "@/components/mode-toggle";

const NavbarRoutes = () => {
  const { userId } = useAuth();
  const pathName = usePathname();

  const isTeacherPage = pathName?.startsWith("/teacher");
  const isCoursePage = pathName?.includes("/courses");

  return (
    <>
      <div className="flex ml-auto">
        <ModeToggle/>
        {isTeacherPage || isCoursePage ? (
          <Link href="/" className="mr-1">
            <Button size="sm" variant="ghost">
              <LogOut className="h-4 w-4 mr-2" />
              Exit
            </Button>
          </Link>
        ) : isTeacher(userId) ? (
          <Link href="/teacher/courses" className="mr-1">
            <Button size="sm" variant="ghost">
              Teacher mode
            </Button>
          </Link>
        ) : null}
      </div>
      <UserButton afterSignOutUrl="/"/>
    </>
  );
};

export default NavbarRoutes;
