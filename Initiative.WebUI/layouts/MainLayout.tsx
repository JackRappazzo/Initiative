import Header from "../components/Header";
import Footer from "../components/Footer";

interface MainLayoutProps {
    children: React.ReactNode;
  }
  
  const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
    return (
      <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
        <Header />
        <div style={{ display: 'flex', flex: 1 }}>
          <main style={{ flex: 1, padding: '1rem' }}>
            {children}
          </main>
        </div>
        <Footer />
      </div>
    );
  };
  
  export default MainLayout;