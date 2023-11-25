import NavbarRoutes from "./navbar-routes";

const Navbar = () => {
    return (
        <div className="p-4 border-b dark:border-b-slate-500/40 h-full flex items-center shadow-sm">
            <NavbarRoutes/>
        </div>
    );
}
 
export default Navbar;