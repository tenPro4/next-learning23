"use client"

import {Layout,Compass,List} from "lucide-react"
import { usePathname } from "next/navigation"
import SidebarItem from "./sidebar-item"

const guestRoutes = [
    {
        icon:Layout,
        label:"Dashboard",
        href:"/"
    },
    {
        icon:Compass,
        label:"Browser",
        href:"/search"
    }
]

const teacherRoutes = [
    {
        icon:List,
        label:"Courses",
        href:"/teacher/courses",
    },
]

const SidebarRoutes = () => {
    const pathName = usePathname()

    const isTeacherPage = pathName?.includes("/teacher");
    const routes = isTeacherPage ? teacherRoutes : guestRoutes

    return (
        <div className="flex flex-col w-full">
            {routes.map(route => (
                <SidebarItem
                key={route.href}
                icon={route.icon}
                href={route.href}
                label={route.label}
                />
            ))}
        </div>
    );
}
 
export default SidebarRoutes;