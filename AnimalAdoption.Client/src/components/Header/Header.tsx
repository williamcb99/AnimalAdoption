import styles from "./Header.module.css";
import { NavLink } from "react-router-dom";

export const Header = () => {
    return (
        <header>
            <nav>
                <NavLink 
                    className={({ isActive }) => isActive ? styles.active : undefined} 
                    to="/"
                    >
                        Home
                </NavLink>
                <NavLink 
                    className={({ isActive }) => isActive ? styles.active : undefined} 
                    to="/available-animals"
                    >
                        Available animals
                </NavLink>
                <NavLink 
                    className={({ isActive }) => isActive ? styles.active : undefined} 
                    to="/about-us"
                    >
                        About us
                </NavLink>
                <NavLink 
                    className={({ isActive }) => isActive ? styles.active : undefined} 
                    to="/admin"
                    >
                        Admin
                </NavLink>
            </nav>
        </header>
    );
}