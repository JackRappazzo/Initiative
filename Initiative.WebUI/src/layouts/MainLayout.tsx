import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import Footer from "../components/Footer";
import SidebarMenu from "../components/SidebarMenu";

const MainLayout: React.FC = () => {
    return (
        <div className="app-container">
            <Header />
            <main>
                <Outlet />
            </main>
            <SidebarMenu />
            <Footer />
        </div>
    );
};

export default MainLayout;